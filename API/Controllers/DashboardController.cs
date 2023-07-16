using API.Dtos;
using API.Services;
using AppCore.Models;
using MainData.Entities;
using MainData.Middlewares;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers;

[Authorize(new[] { UserRole.Admin })]
public class DashboardController : BaseController
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }
    
    [HttpGet]
    [SwaggerOperation("Get basic information")]
    public async Task<ApiResponse<DashboardDto>> GetEvents()
    {
        return await _dashboardService.GetBaseInformation();
    }
}