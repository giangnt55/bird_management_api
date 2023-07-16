using MainData.Entities;
using MainData.Middlewares;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/[Controller]")]
[Authorize(new[] { UserRole.Admin , UserRole.Member, UserRole.Staff, UserRole.Manager})]
public class BaseController : ControllerBase
{
    
}