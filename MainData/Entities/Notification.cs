using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class Notification : BaseEntity
{
    public NotificationType Type { get; set; }
    public NotificationStatus Status { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime? PublishAt { get; set; }
    public bool IsSendAll { get; set; }
    public Guid? UserId { get; set; } 
    
    //Relationship
    public virtual User? User { get; set; } 
}

public enum NotificationType
{
    Information = 1,
    Event = 2,
    System = 3
}

public enum NotificationStatus
{
    Waiting = 1,
    Sent = 2,
    Cancel = 3,
}

public class NotificationConfig : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.Property(x => x.Title).IsRequired();
        builder.Property(x => x.Content).IsRequired();
        builder.Property(x => x.Type).IsRequired();
        builder.Property(x => x.Status).IsRequired();
        builder.Property(x => x.PublishAt).IsRequired();
        builder.Property(x => x.IsSendAll).IsRequired().HasDefaultValue(false);
        builder.Property(x => x.UserId).HasDefaultValue(Guid.Empty);
        builder.HasOne(x => x.User);
    }
}