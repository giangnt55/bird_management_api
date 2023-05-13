using AppCore.Models;
using MainData.Middlewares;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class TestController : BaseController
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ApiResponse> Hello()
    {
        return ApiResponse.Success("Hello");
    }
}