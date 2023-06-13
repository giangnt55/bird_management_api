using API.Dtos;
using API.Services;
using AppCore.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers
{
    public class NotificationController : BaseController
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }
        [HttpGet]
        [SwaggerOperation("Get list ...")]
        public async Task<ApiResponses<NotificationDto>> GetAllNotifications([FromQuery] NotificationQueryDto queryDto)
        {
            return await _notificationService.GetNotifications(queryDto);
        }
        [HttpGet("{id}")]
        [SwaggerOperation("Detail ...")]
        public async Task<ApiResponse> GetNewsDetail(Guid id)
        {
            return await _notificationService.GetNotificationId(id);
        }


        [HttpPost]
        [SwaggerOperation("Push new notification")]
        public async Task<ApiResponse<DetailNotificationDto>> CreateNotification(CreateNotification notificationDto)
        {
            return await _notificationService.CreateNotification(notificationDto);
        }

    }
}
