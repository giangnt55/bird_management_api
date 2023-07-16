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
  Task<ApiResponse> ResetPasswordByEmail(ResetPasswordDto email);
  Task<ApiResponse> ResetPasswordByCode(UpdatePasswordDto updatePasswordDto);
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
      Email = user.Username,
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
    var existedEmail = await MainUnitOfWork.UserRepository.FindAsync(new Expression<Func<User, bool>>[]
    {
            x => !x.DeletedAt.HasValue,
            x => x.Email == registerDto.Email
    }, null);

    if (existedEmail.Any())
      throw new ApiException("This email has been used", StatusCode.BAD_REQUEST);
    
    var existedUsername = await MainUnitOfWork.UserRepository.FindAsync(new Expression<Func<User, bool>>[]
    {
      x => !x.DeletedAt.HasValue,
      x => x.Username == registerDto.Username
    }, null);
    
    if (existedUsername.Any())
      throw new ApiException("This username has been used", StatusCode.BAD_REQUEST);
    

    var user = registerDto.ProjectTo<RegisterDto, User>();
    var salt = SecurityExtension.GenerateSalt();
    user.Password = SecurityExtension.HashPassword<User>(registerDto.Password, salt);
    user.Status = UserStatus.Active;
    user.Role = UserRole.Member;
    user.Salt = salt;
    user.Email = user.Email?.ToLower();
    // string[] emailParts = registerDto.Email.Split('@'); // Tách địa chỉ email thành mảng các phần tử dựa trên ký tự "@"
    // user.Username = emailParts[0]; ; // Gán gmail cho username
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

  public async Task<ApiResponse> ResetPasswordByEmail(ResetPasswordDto email)
  {
    var user = MainUnitOfWork.UserRepository.GetQuery().Where(x => x.Email == email.Email).SingleOrDefault();
    if (user == null)
    {
      throw new ApiException("User not found", StatusCode.NOT_FOUND);
    }

    // Generate a random reset code
    var resetCode = GenerateRandomCode(6);
    // Create a reset token
    var claims = SetClaims(user!);
    var accessExpiredAt = CurrentDate.AddMinutes(EnvironmentExtension.GetJwtAccessTokenExpires());
    var resetToken = new Token
    {
      Id = Guid.NewGuid(),
      UserId = user.Id,
      Type = TokenType.ResetPassword,
      AccessToken = JwtExtensions.GenerateAccessToken(claims, accessExpiredAt),
      RefreshToken = resetCode,
      AccessExpiredAt = DateTime.Now.AddMinutes(30),
      RefreshExpiredAt = DateTime.Now.AddMinutes(30),
      Status = TokenStatus.Active
    };
    var token = MainUnitOfWork.TokenRepository.GetQuery().Where(x => x.UserId == user.Id && x.Type == TokenType.ResetPassword).SingleOrDefault();
    if (token != null)
    {
      token = resetToken;
      if (!await MainUnitOfWork.TokenRepository.InsertAsync(token, user.Id, CurrentDate))
      {
        throw new ApiException(MessageKey.ServerError, StatusCode.SERVER_ERROR);
      }

    }
    // Save the reset token to the database
    if (!await MainUnitOfWork.TokenRepository.InsertAsync(resetToken, user.Id, CurrentDate))
    {
      throw new ApiException(MessageKey.ServerError, StatusCode.SERVER_ERROR);
    }
    // Send the reset code via email
    MailExtension _mailExtension = new MailExtension();
    var resetEmailBody = $"Your reset code is: {resetCode}";
    _mailExtension.SendMail(user.Fullname, user.Email, resetEmailBody);

    return ApiResponse.Success();
  }

  public async Task<ApiResponse> ResetPasswordByCode(UpdatePasswordDto resetPasswordDto)
  {
    var user = MainUnitOfWork.UserRepository.GetQuery().Where(x => x.Email == resetPasswordDto.Email).SingleOrDefault();
    if (user == null)
    {
      throw new ApiException("User not found", StatusCode.NOT_FOUND);
    }

    // Retrieve the reset token from the database based on the user and reset code
    var resetToken = MainUnitOfWork.TokenRepository.GetQuery().Where(x => x.UserId == user.Id && x.RefreshToken == resetPasswordDto.resetCode).SingleOrDefault();

    if (resetToken == null)
    {
      throw new ApiException("Invalid reset code", StatusCode.BAD_REQUEST);
    }

    if (resetToken.RefreshExpiredAt < DateTime.Now)
    {
      throw new ApiException("Reset token expired", StatusCode.UNAUTHORIZED);
    }

    // Update the user's password
    var updateUser = resetPasswordDto.ProjectTo<UpdatePasswordDto, User>();
    var salt = SecurityExtension.GenerateSalt();
    updateUser.Password = SecurityExtension.HashPassword<User>(resetPasswordDto.NewPassword, salt);
    updateUser.Status = UserStatus.Active;
    updateUser.Role = UserRole.Member;
    updateUser.Salt = salt;
    updateUser.Username = user.Username;
    if (!await MainUnitOfWork.UserRepository.UpdateAsync(user, AccountId, CurrentDate))
      throw new ApiException("Update Password fail", StatusCode.SERVER_ERROR);

    // Invalidate the reset token

    return ApiResponse.Success();
  }

  private string GenerateRandomCode(int length)
  {
    var random = new Random();
    var code = string.Empty;
    for (int i = 0; i < length; i++)
    {
      code += random.Next(0, 9).ToString();
    }
    return code;
  }

}
