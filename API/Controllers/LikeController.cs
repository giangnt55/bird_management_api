using API.Dtos;
using API.Services;
using AppCore.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers
{
    public class LikeController : BaseController
    {
        private readonly ILikeService _service;

        public LikeController(ILikeService likeService)
        {
            _service = likeService;
        }

        [HttpPost]
        [SwaggerOperation("Like post")]
        public async Task<ApiResponse> LikePost(LikePostDto likeDto)
        {
            return await _service.LikePost(likeDto);
        }

        [HttpPost("comment")]
        [SwaggerOperation("Like comment")]
        public async Task<ApiResponse> LikeComment(LikeCommentDto likeDto)
        {
            return await _service.LikeComment(likeDto);
        }
    }
}
