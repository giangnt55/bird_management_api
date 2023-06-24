using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class FeedBack : BaseEntity
{
    public Guid ParticipantId { get; set; }
    public Guid EventId { get; set; }
    public string? Content { get; set; }
    public RateStar? Rating { get; set; }
    
    //Relationship
    public virtual Event? Event { get; set; }
    public virtual Participant? Participant { get; set; }
}
public enum RateStar
{
    Excellent = 5, Good = 4, Normal = 3, Bad = 2, VeryBad=1
}
public class FeedBackConfig : IEntityTypeConfiguration<FeedBack>
{
    public void Configure(EntityTypeBuilder<FeedBack> builder)
    {
        builder.Property(x => x.ParticipantId).IsRequired();
        builder.Property(x => x.EventId).IsRequired();
        builder.Property(x => x.Content).IsRequired(false);
        builder.Property(x => x.Rating).IsRequired(false);
        
        //Relationship
        builder.HasOne(x => x.Event)
            .WithMany(x => x.FeedBacks)
            .HasForeignKey(x => x.EventId);
        
        builder.HasOne(x => x.Participant)
            .WithMany(x => x.FeedBacks)
            .HasForeignKey(x => x.ParticipantId);
    }
}