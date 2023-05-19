using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class Activity : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Location { get; set; } = string.Empty;
    public int MaxParticipants { get; set; }
    public ActivityStatus Status { get; set; }
    public ActivityType Type { get; set; }
    
    //Relationship
    public User Host { get; set; } = new User();
    public IEnumerable<ActivityFeedBack> ActivityFeedBacks { get; set; } = new List<ActivityFeedBack>();
    public IEnumerable<Attendance> Attendances { get; set; } = new List<Attendance>();
    public IEnumerable<Post> Posts { get; set; } = new List<Post>();
}

public enum ActivityStatus
{
    Upcoming = 1, Happening = 2, Over = 3
}

public enum ActivityType
{
    Online = 1, Offline = 2
}

public class ActivityConfig : IEntityTypeConfiguration<Activity>
{
    public void Configure(EntityTypeBuilder<Activity> builder)
    {
        builder.Property(x => x.Title).IsRequired();
        builder.Property(x => x.Description).IsRequired();
        builder.Property(x => x.Date).IsRequired();
        builder.Property(x => x.Location).IsRequired();
        builder.Property(x => x.MaxParticipants).IsRequired();
        builder.Property(x => x.Status).IsRequired();
        builder.Property(x => x.Type).IsRequired();
        builder.HasOne(x => x.Host);
        builder.HasMany(x => x.ActivityFeedBacks).WithOne(x => x.Activity)
            .HasForeignKey(x => x.ActivityId);
        builder.HasMany(x => x.Attendances).WithOne(x => x.Activity)
            .HasForeignKey(x => x.ActivityId);
        builder.HasMany(x => x.Posts).WithOne(x => x.Activity)
            .HasForeignKey(x => x.ActivityId);
    }
}