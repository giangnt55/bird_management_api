using API.Dtos;
using API.Services;
using AppCore.Models;
using MainData.Middlewares;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers;

public class AuthController : BaseController
{
    private readonly IAuthenticationService _authenticationService;

    public AuthController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [HttpPost("sign-in")]
    [AllowAnonymous]
    [SwaggerOperation("Login api")]
    public async Task<ApiResponse<AuthDto>> SignIn(AccountCredentialLoginDto accountCredentialLoginDto)
    {
        return await _authenticationService.SignIn(accountCredentialLoginDto);
    }
    
    [HttpPost("sign-out")]
    public async Task<ApiResponse> Logout()
    {
        return await _authenticationService.RevokeToken();
    }
    
    [HttpPost("refresh-token")]
    [AllowAnonymous]
    [SwaggerOperation("Refresh token")]
    public async Task<ApiResponse<AuthDto>> SignIn(AuthRefreshDto authRefreshDto)
    {
        return await _authenticationService.RefreshToken(authRefreshDto);
    }
    
    [HttpPost("register")]
    [AllowAnonymous]
    [SwaggerOperation("Register account")]
    public async Task<ApiResponse> Register(RegisterDto registerDto)
    {
        return await _authenticationService.Register(registerDto);
    }
}