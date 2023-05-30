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
}