using System.Linq.Expressions;
using System.Security.Claims;
using API.Dtos;
using AppCore.Models;
using MainData;
using MainData.Entities;
using AppCore.Extensions;
using MainData.Repositories;

namespace API.Services;

public interface IAuthenticationService : IBaseService
{
    Task<ApiResponse<AuthDto>> SignIn(AccountCredentialLoginDto accountCredentialLoginDto);
    Task<ApiResponse<AuthDto>> RefreshToken(AuthRefreshDto authRefreshDto);
    Task<ApiResponse> Register(RegisterDto registerDto);
    Task<ApiResponse> RevokeToken();
}

public class AuthenticationService : BaseService, IAuthenticationService
{
    public AuthenticationService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
    {
    }
    public async Task<ApiResponse<AuthDto>> SignIn(AccountCredentialLoginDto accountCredentialLoginDto)
    {
        var user = await MainUnitOfWork.UserRepository.FindOneAsync(new Expression<Func<User, bool>>[]
        {
            x => !x.DeletedAt.HasValue && x.Username == accountCredentialLoginDto.Username
        });

        if (user == null)
        {
            throw new ApiException(StatusCode.NOT_FOUND);
        }

        // Check status
        if (user.Status == UserStatus.InActive)
            throw new ApiException(MessageKey.AccountNotActivated, StatusCode.NOT_ACTIVE);

        // Check password
        if (!accountCredentialLoginDto.Password.VerifyPassword<User>(user.Salt, user.Password))
        {
            throw new ApiException("Invalid username or password", StatusCode.BAD_REQUEST);
        }

        var claims = SetClaims(user);
        var accessExpiredAt = CurrentDate.AddMinutes(EnvironmentExtension.GetJwtAccessTokenExpires());
        var refreshExpiredAt = CurrentDate.AddMinutes(EnvironmentExtension.GetJwtResetTokenExpires());
        var accessToken = JwtExtensions.GenerateAccessToken(claims, accessExpiredAt);
        var refreshToken = JwtExtensions.GenerateRefreshToken();

        var token = new Token
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Type = TokenType.SignInToken,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessExpiredAt = accessExpiredAt,
            RefreshExpiredAt = refreshExpiredAt,
            Status = TokenStatus.Active
        };

        if (!await MainUnitOfWork.TokenRepository.InsertAsync(token, user.Id, CurrentDate))
            throw new ApiException(MessageKey.ServerError, StatusCode.SERVER_ERROR);
        //
        var verifyResponse = new AuthDto
        {
            AccessExpiredAt = accessExpiredAt,
            RefreshExpiredAt = refreshExpiredAt,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Email = user .Username,
            Fullname = user.Fullname,
            Role = user.Role,
            UserId = user.Id,
            //IsFirstLogin = (user.FirstLoginAt == null)
        };

        return ApiResponse<AuthDto>.Success(verifyResponse);
    }

    public async Task<ApiResponse<AuthDto>> RefreshToken(AuthRefreshDto authRefreshDto)
    {
        var token = await MainUnitOfWork.TokenRepository.FindOneAsync(new Expression<Func<Token, bool>>[]
        {
            t => t.RefreshToken == authRefreshDto.RefreshToken,
            t => t.Type == TokenType.SignInToken
        });

        if (token == null)
            throw new ApiException("Not found", StatusCode.NOT_FOUND);

        var account = await MainUnitOfWork.UserRepository.FindOneAsync(token.UserId);
        if (account != null && account.Status == UserStatus.InActive)
            throw new ApiException("Not found", StatusCode.NOT_FOUND);

        if (Math.Abs((token.AccessExpiredAt - CurrentDate).TotalMinutes) < 1)
            throw new ApiException(MessageKey.TokenIsStillValid, StatusCode.BAD_REQUEST);

        var claims = SetClaims(account!);
        var accessExpiredAt = CurrentDate.AddMinutes(EnvironmentExtension.GetJwtAccessTokenExpires());
        var refreshExpiredAt = CurrentDate.AddMinutes(EnvironmentExtension.GetJwtResetTokenExpires());
        var accessToken = JwtExtensions.GenerateAccessToken(claims, accessExpiredAt);
        var refreshToken = JwtExtensions.GenerateRefreshToken();

        token.AccessToken = accessToken;
        token.RefreshToken = refreshToken;
        token.AccessExpiredAt = accessExpiredAt;
        token.RefreshExpiredAt = refreshExpiredAt;
        token.Status = TokenStatus.Active;

        if (!await MainUnitOfWork.TokenRepository.UpdateAsync(token, account!.Id, CurrentDate))
            throw new ApiException(MessageKey.ServerError, StatusCode.SERVER_ERROR);

        // Update current device token for push notify
        return ApiResponse<AuthDto>.Success(new AuthDto
        {
            AccessExpiredAt = accessExpiredAt,
            RefreshExpiredAt = refreshExpiredAt,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Email = account.Email,
            Fullname = account.Fullname,
            Role = account.Role,
            UserId = account.Id,
            IsFirstLogin = false // Not need at the current
        });
    }

    public async Task<ApiResponse> Register(RegisterDto registerDto)
    {
        var existed = await MainUnitOfWork.UserRepository.FindAsync(new Expression<Func<User, bool>>[]
        {
            x => !x.DeletedAt.HasValue,
            x => x.Email == registerDto.Email
        }, null);

        if (existed.Any())
            throw new ApiException("This email has been used", StatusCode.BAD_REQUEST);

        var user = registerDto.ProjectTo<RegisterDto, User>();
        var salt = SecurityExtension.GenerateSalt();
        user.Password = SecurityExtension.HashPassword<User>(registerDto.Password, salt);
        user.Status = UserStatus.Active;
        user.Role = UserRole.Member;
        user.Salt = salt;

        if (!await MainUnitOfWork.UserRepository.InsertAsync(user, Guid.Empty, CurrentDate))
            throw new ApiException("Register fail", StatusCode.SERVER_ERROR);

        return ApiResponse.Success();
    }

    public async Task<ApiResponse> RevokeToken()
    {
        //Get token from header
        var bearToken = string.Empty;
        if (HttpContextAccessor.HttpContext != null)
        {
            bearToken = HttpContextAccessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault()
                ?.Split(" ").Last();
        }

        // Find token
        var token = await MainUnitOfWork.TokenRepository.FindOneAsync(new Expression<Func<Token, bool>>[]
        {
            x => x.AccessToken == bearToken,
            x => !x.DeletedAt.HasValue
        });

        //Find account
        var account = await MainUnitOfWork.UserRepository.FindOneAsync(new Expression<Func<User, bool>>[]
        {
            x => x.Id == token!.UserId,
            x => !x.DeletedAt.HasValue
        });

        if (account == null || token == null)
            throw new ApiException(MessageKey.TokenInCorrect, StatusCode.BAD_REQUEST);

        // Update - delete token
        token.AccessExpiredAt = CurrentDate;
        token.RefreshExpiredAt = CurrentDate;

        if (!(await MainUnitOfWork.TokenRepository.DeleteAsync(token, null, CurrentDate)))
        {
            throw new ApiException(MessageKey.ServerError, StatusCode.SERVER_ERROR);
        }

        //account.LastLoginAt = CurrentDate;
        // Update account
        if (!(await MainUnitOfWork.UserRepository.UpdateAsync(account, account.Id, CurrentDate)))
        {
            throw new ApiException(MessageKey.ServerError, StatusCode.SERVER_ERROR);
        }

        return ApiResponse.Success();
    }

    private IEnumerable<Claim> SetClaims(User account)
    {
        // Create token
        var claims = new Claim[]
        {
            new(AppClaimTypes.Id, account.Id.ToString()),
            new(AppClaimTypes.Role, account.Role.ToString()),
            new(AppClaimTypes.Status, account.Status.ToString()),
        }.ToList();
        return claims;
    }

}
