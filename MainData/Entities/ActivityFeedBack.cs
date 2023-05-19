using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class ActivityFeedBack : BaseEntity
{
    public Guid ActivityId { get; set; }
    public int? Rating { get; set; }
    public string Content { get; set; } = string.Empty;
    
    //Relationship
    public virtual Activity Activity { get; set; } = new Activity();
    public virtual User User { get; set; } = new User();
}

public class ActivityFeedBackConfig : IEntityTypeConfiguration<ActivityFeedBack>
{
    public void Configure(EntityTypeBuilder<ActivityFeedBack> builder)
    {
        builder.Property(x => x.ActivityId).IsRequired();
        builder.Property(x => x.Rating).IsRequired(false);
        builder.Property(x => x.Content).IsRequired();
        builder.HasOne(x => x.Activity);
        builder.HasOne(x => x.User);
    }
}