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
        public async Task<ApiResponse> Comment(Guid postId, CommentCreateDto commentDto)
        {
            return await _service.AddComment(postId, commentDto);
        }

        [HttpPut]
        [SwaggerOperation("Update comment")]
        public async Task<ApiResponse> UpdateComment(Guid id, CommentCreateDto commentDto)
        {
            return await _service.UpdateComment(id, commentDto);
        }

        [HttpDelete]
        [SwaggerOperation("Delete comment")]
        public async Task<ApiResponse> DeleteComment(Guid id)
        {
            return await _service.DeleteComment(id);
        }

        [HttpGet("post/{postId:guid}")]
        [SwaggerOperation("Get comment by post Id")]
        public async Task<ApiResponses<CommentDto>> GetCommentByPost(Guid postId)
        {
          return await _service.GetCommentByPost(postId);
        }
    }
}
