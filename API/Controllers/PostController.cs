using API.Dtos;
using API.Services;
using AppCore.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers
{
    public class PostController : BaseController
    {
        private readonly IPostService _service;

        public PostController(IPostService service)
        {
            _service = service;
        }

        [HttpGet]
        [SwaggerOperation("Get list ...")]
        public async Task<ApiResponses<PostDto>> GetPosts(PostQueryDto postDto)
        {
            return await _service.GetPosts(postDto);
        }

        [HttpPost]
        [SwaggerOperation("Insert ...")]
        public async Task<ApiResponse> InsertPost(PostCreateDto postDto)
        {
            return await _service.InsertPost(postDto);
        }

        [HttpDelete]
        [SwaggerOperation("Delete ...")]
        public async Task<ApiResponse> DeletePost(PostDeleteDto postDto)
        {
            return await _service.DeletePost(postDto);
        }
    }
}
