using API.Dtos;
using API.Services;
using AppCore.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers
{
    public class ParticipantController : BaseController
    {
        private readonly IParticipantService _service;

        public ParticipantController(IParticipantService service)
        {
            _service = service;
        }

        [HttpGet]
        [SwaggerOperation("Get list participant")]
        public async Task<ApiResponses<ParticipantDto>> GetAllParticipant([FromQuery] ParticipantQueryDto queryDto)
        {
            return await _service.GetParticipants(queryDto);
        }

        [HttpPost]
        [SwaggerOperation("Join event")]
        public async Task<ApiResponse> InsertParticipant(ParticipantCreateDto participantDto)
        {
            return await _service.Create(participantDto);
        }

        [HttpPut]
        [SwaggerOperation("Update participant")]
        public async Task<ApiResponse> UpdateParticipant(Guid Id, ParticipantUpdateDto participantDto)
        {
            return await _service.UpdateParticipant(Id, participantDto);
        }

        [HttpDelete("{id:guid}")]
        [SwaggerOperation("Delete participant")]
        public async Task<ApiResponse> Delete(Guid id)
        {
            return await _service.Delete(id);
        }
    }
}
