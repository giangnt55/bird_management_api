using API.Dtos;
using API.Services;
using AppCore.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers
{
    public class FeedbackController : BaseController
    {
        private readonly IFeedbackService _service;

        public FeedbackController(IFeedbackService service)
        {
            _service = service;
        }

        [HttpGet]
        [SwaggerOperation("Get feedbacks")]
        public async Task<ApiResponses<FeedbackDto>> GetFeedbacks([FromQuery] FeedbackQueryDto queryDto)
        {
            return await _service.GetListFeedbacks(queryDto);
        }

        [HttpPost]
        [SwaggerOperation("Create new feedback")]
        public async Task<ApiResponse> InsertFeedback(FeedbackCreateDto feedbackDto)
        {
            return await _service.Create(feedbackDto);
        }

        [HttpGet("{id:guid}")]
        [SwaggerOperation("Get detail feedback information")]
        public async Task<ApiResponse<FeedbackDetailDto>> Get(Guid id)
        {
            return await _service.GetFeedback(id);
        }

        [HttpPut("{id:guid}")]
        [SwaggerOperation("Update feedback information")]
        public async Task<ApiResponse<FeedbackDetailDto>> Update(Guid id, FeedbackUpdateDto feedbackUpdateDto)
        {
            return await _service.Update(id, feedbackUpdateDto);
        }

        [HttpDelete("{id:guid}")]
        [SwaggerOperation("Delete feedback")]
        public async Task<ApiResponse> Delete(Guid id)
        {
            return await _service.Delete(id);
        }
    }
}
