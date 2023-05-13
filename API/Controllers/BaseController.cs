using MainData.Entities;
using MainData.Middlewares;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/[Controller]")]
[Authorize(new[] { UserRole.Admin })]
public class BaseController : ControllerBase
{
    
}