using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class Notification : BaseEntity
{
    public Guid UserId { get; set; }
    public NotificationType Type { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    
    //Relationship
    public virtual User User { get; set; } = new User();
}

public enum NotificationType
{
    Information = 1, Warning = 2
}

public class NotificationConfig : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.Type).IsRequired();
        builder.Property(x => x.Content).IsRequired();
        builder.Property(x => x.Date).IsRequired();
        builder.Property(x => x.IsRead).IsRequired();
        builder.Property(x => x.ReadAt).IsRequired(false);
        
        //Relationship
        builder.HasOne(x => x.User)
            .WithMany(x => x.Notifications)
            .HasForeignKey(x => x.UserId);
    }
}