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

    [HttpPost]
    [SwaggerOperation("Insert ...")]
    public async Task<ApiResponse<bool>> InsertBird(BirdCreateDto birdDto)
    {
        return await _birdService.InsertBird(birdDto);
    }

    [HttpPatch]
    [SwaggerOperation("Update ...")]
    public async Task<ApiResponse<bool>> UpdateBird(BirdUpdateDto birdDto)
    {
        return await _birdService.UpdateBird(birdDto);
    }

    [HttpDelete]
    [SwaggerOperation("Delete ...")]
    public async Task<ApiResponse<bool>> DeleteBird(BirdDeleteDto birdDto)
    {
        return await _birdService.DeleteBird(birdDto);
    }
}