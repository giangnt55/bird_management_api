using API.Dtos;
using API.Services;
using AppCore.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
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

        [HttpPut]
        [SwaggerOperation("Update comment")]
        public async Task<ApiResponse> UpdateComment(Guid id, CommentCreateDto commentDto)
        {
            return await _service.UpdateComment(id, commentDto);
        }

        [HttpDelete]
        [SwaggerOperation("Delete comment")]
        public async Task<ApiResponse> DeleteComment(Guid id, CommentDeleteDto commentDto)
        {
            return await _service.DeleteComment(id, commentDto);
        }
    }
}
