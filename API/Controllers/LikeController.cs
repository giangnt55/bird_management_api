using API.Dtos;
using API.Services;
using AppCore.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers;

public class LikeController : BaseController
{
  private readonly ILikeService _likeService;

  public LikeController(ILikeService likeService)
  {
    _likeService = likeService;
  }

  [HttpPost]
  [SwaggerOperation("Create like")]
  public async Task<ApiResponse> CreateLike([FromBody] CreateLikeDto createLikeDto)
  {
    return await _likeService.CreateLike(createLikeDto);
  }

  [HttpDelete]
  [SwaggerOperation("delete like")]
  public async Task<ApiResponse> UnLike([FromBody] CreateLikeDto createLikeDto)
  {
    return await _likeService.Unlike(createLikeDto);
  }

  [HttpGet("post/{postId:guid}")]
  [SwaggerOperation("Create like")]
  public async Task<ApiResponse> GetLikesOfPost(Guid postId)
  {
    return await _likeService.GetLikeOfPost(postId);
  }
}
