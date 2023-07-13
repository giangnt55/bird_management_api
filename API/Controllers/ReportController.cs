using API.Dtos;
using API.Services;
using AppCore.Data;
using AppCore.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers;

public class ReportController: BaseController
{
    private readonly IReportService _reportService;

    public ReportController(IReportService reportService)
    {
        _reportService = reportService;
    }
    
    
    [HttpPost]
    [SwaggerOperation("Create report")]
    public async Task<ApiResponse> CreateReport(ReportCreateDto reportCreateDto)
    {
        return await _reportService.CreateReport(reportCreateDto);
    }

    [HttpDelete]
    [SwaggerOperation("Cancel report")]
    public async Task<ApiResponse> CancelReport(ReportCreateDto reportCreateDto)
    {
        return await _reportService.CancelReport(reportCreateDto);
    }
}