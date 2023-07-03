using API.Dtos;
using API.Services;
using AppCore.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers
{
    public class FollowerController : BaseController
    {
        private readonly IFollowerService _service;

        public FollowerController(IFollowerService service)
        {
            _service = service;
        }

        [HttpGet]
        [SwaggerOperation("Get followers")]
        public async Task<ApiResponses<FollowerDetailDto>> GetFollowers([FromQuery] FollowerQuery queryDto)
        {
            return await _service.GetFollowerOfUser(queryDto);
        }

        [HttpPost]
        [SwaggerOperation("Create new follower")]
        public async Task<ApiResponse> InsertFollower(FollowerCreateDto followerDto)
        {
            return await _service.CreateFollower(followerDto);
        }

        [HttpDelete("{id:guid}")]
        [SwaggerOperation("Delete follower")]
        public async Task<ApiResponse> Delete(Guid id)
        {
            return await _service.UnFollower(id);
        }
    }
}
