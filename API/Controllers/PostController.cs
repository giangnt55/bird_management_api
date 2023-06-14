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
        [SwaggerOperation("Get posts")]
        public async Task<ApiResponses<PostDto>> GetPosts([FromQuery] PostQueryDto queryDto)
        {
            return await _service.GetPosts(queryDto);
        }

        [HttpPost]
        [SwaggerOperation("Create new post")]
        public async Task<ApiResponse> InsertPost(PostCreateDto postDto)
        {
            return await _service.Create(postDto);
        }

        [HttpGet("{id:guid}")]
        [SwaggerOperation("Get detail post information")]
        public async Task<ApiResponse<DetailPostDto>> Get(Guid id)
        {
            return await _service.GetPost(id);
        }
        
        [HttpPut("{id:guid}")]
        [SwaggerOperation("Update post information")]
        public async Task<ApiResponse<DetailPostDto>> Update(Guid id, PostUpdateDto postUpdateDto)
        {
            return await _service.Update(id, postUpdateDto);
        }
        
        [HttpDelete("{id:guid}")]
        [SwaggerOperation("Delete post")]
        public async Task<ApiResponse> Delete(Guid id)
        {
            return await _service.Delete(id);
        }
    }
}
