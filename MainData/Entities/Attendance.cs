using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class Attendance : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid ActivityId { get; set; }
    public DateTime? CheckIn { get; set; }
    public DateTime? CheckOut { get; set; }
    
    //Relationship
    public virtual User User { get; set; } = new User();
    public virtual Activity Activity { get; set; } = new Activity();
}

public class AttendanceConfig : IEntityTypeConfiguration<Attendance>
{
    public void Configure(EntityTypeBuilder<Attendance> builder)
    {
        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.ActivityId).IsRequired();
        builder.Property(x => x.CheckIn).IsRequired(false);
        builder.Property(x => x.CheckOut).IsRequired(false);
        builder.HasOne(x => x.User);
        builder.HasOne(x => x.Activity);
    }
}

