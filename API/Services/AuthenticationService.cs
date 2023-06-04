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