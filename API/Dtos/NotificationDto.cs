using AppCore.Models;
using MainData.Entities;

namespace API.Dtos
{
    public class NotificationDto : BaseDto
    {
        public Guid UserId { get; set; }
        public NotificationType Type { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }

        //Relationship
        public virtual User? User { get; set; }
    }

    public class CreateNotification
    {
        public Guid UserId { get; set; }
        public NotificationType Type { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
    }
    public class UpdateNotification
    {
        public Guid UserId { get; set; }
        public NotificationType Type { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
    }
    public class DetailNotificationDto: BaseDto
    {
        public Guid UserId { get; set; }
        public NotificationType Type { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }

    }

    public class NotificationQueryDto : BaseQueryDto
    {
        public string Content { get; set; } = string.Empty;
        public bool IsRead { get; set; }

    }
}
