using API.Dtos;
using API.Services;
using AppCore.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers
{
    public class EventController : BaseController
    {
        private readonly IEventService _service;

        public EventController(IEventService service)
        {
            _service = service;
        }

        [HttpGet]
        [SwaggerOperation("Get events")]
        public async Task<ApiResponses<EventDto>> GetEvents([FromQuery] EventQueryDto queryDto)
        {
            return await _service.GetListEvents(queryDto);
        }

        [HttpPost]
        [SwaggerOperation("Create new event")]
        public async Task<ApiResponse> InsertEvent([FromBody]EventCreateDto eventDto)
        {
            return await _service.Create(eventDto);
        }

        [HttpGet("{id:guid}")]
        [SwaggerOperation("Get detail event information")]
        public async Task<ApiResponse<EventDetailDto>> Get(Guid id)
        {
            return await _service.GetEvent(id);
        }

        [HttpPut("{id:guid}")]
        
        [SwaggerOperation("Update event information")]
        public async Task<ApiResponse<EventDetailDto>> Update(Guid id, EventUpdateDto eventUpdateDto)
        {
            return await _service.Update(id, eventUpdateDto);
        }

        [HttpDelete("{id:guid}")]
        [SwaggerOperation("Delete event")]
        public async Task<ApiResponse> Delete(Guid id)
        {
            return await _service.Delete(id);
        }
    }
}
