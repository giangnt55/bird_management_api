using API.Services;
using AppCore.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LikeController : ControllerBase
    {
        private readonly ILikeService _service;

        public LikeController(ILikeService likeService)
        {
            _service = likeService;
        }

        [HttpPost("{id:guid}")]
        [SwaggerOperation("Like post")]
        public async Task<ApiResponse> LikePost(Guid id)
        {
            return await _service.LikePost(id);
        }
    }
}
