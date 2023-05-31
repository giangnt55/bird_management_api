using API.Dtos;
using API.Services;
using AppCore.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers;

public class BirdController : BaseController
{
    private readonly IBirdService _birdService;

    public BirdController(IBirdService birdService) 
    {
        _birdService = birdService;
    }
    
    [HttpGet]
    [SwaggerOperation("Get list ...")]
    public async Task<ApiResponses<BirdDto>> GetListBird([FromQuery]BirdQueryDto queryDto)
    {
        return await _birdService.GetBird(queryDto);
    }
}