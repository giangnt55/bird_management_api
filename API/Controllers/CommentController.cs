using API.Dtos;
using API.Services;
using AppCore.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers
{
  public class CommentController : BaseController
  {
    private readonly ICommentService _service;

    public CommentController(ICommentService service)
    {
      _service = service;
    }

    [HttpPost]
    [SwaggerOperation("Comment")]
    public async Task<ApiResponse> Comment(CommentCreateDto commentDto)
    {
      return await _service.AddComment(commentDto);
    }

    // [HttpPut]
    // [SwaggerOperation("Update comment")]
    // public async Task<ApiResponse> UpdateComment(Guid id, CommentCreateDto commentDto)
    // {
    //     return await _service.UpdateComment(id, commentDto);
    // }

    // [HttpDelete]
    // [SwaggerOperation("Delete comment")]
    // public async Task<ApiResponse> DeleteComment(Guid id, CommentDeleteDto commentDto)
    // {
    //     return await _service.DeleteComment(id, commentDto);
    // }

    [HttpGet("post/{postId:guid}")]
    [SwaggerOperation("Get comment by post Id")]
    public async Task<ApiResponses<CommentDto>> GetCommentByPost(Guid postId)
    {
      return await _service.GetCommentByPost(postId);
    }
  }
}
