using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class Report : BaseEntity
{
    public Guid TargetId { get; set; }
    public ReportType Type { get; set; } 
    
    //Relationship
    public virtual User? User { get; set; }
    public virtual Post? Post { get; set; }
    public virtual Comment? Comment { get; set; }
}

public enum ReportType
{
    Spam = 1,
    Violence = 2,
    UnauthorizedSales = 3,
    Trouble = 4,
    Terrorism = 5,
    FakeInformation = 6,
    Others = 7
}

public class ReportConfig : IEntityTypeConfiguration<Report>
{
    public void Configure(EntityTypeBuilder<Report> builder)
    {
        builder.Property(x => x.TargetId).IsRequired();
        builder.Property(x => x.Type).IsRequired();
        
        //Relationship
        builder.HasOne(x => x.User)
            .WithMany(x => x.Reports)
            .HasForeignKey(x => x.CreatorId);
        
        builder.HasOne(x => x.Comment)
            .WithMany(x => x.Reports)
            .HasForeignKey(x => x.TargetId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(x => x.Post)
            .WithMany(x => x.Reports)
            .HasForeignKey(x => x.TargetId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}