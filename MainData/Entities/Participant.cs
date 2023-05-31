using AppCore.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class Participant : BaseEntity
{
    public Guid EventId { get; set; }
    public ParticipantRole Role { get; set; }
    
    //Relationship
    public virtual User User { get; set; } = new User();
    public virtual Event Event { get; set; } = new Event();
    public virtual IEnumerable<FeedBack> FeedBacks { get; set; } = new List<FeedBack>();
}

public enum ParticipantRole
{
    Participant = 1,
    Staff = 2
}

public class ParticipantConfig : IEntityTypeConfiguration<Participant>
{
    public void Configure(EntityTypeBuilder<Participant> builder)
    {
        builder.Property(x => x.EventId).IsRequired();
        builder.Property(x => x.Role).IsRequired();
        
        //Relationship
        builder.HasOne(x => x.Event)
            .WithMany(x => x.Participants)
            .HasForeignKey(x => x.EventId);
        
        builder.HasOne(x => x.User)
            .WithMany(x => x.Participants)
            .HasForeignKey(x => x.CreatorId);
        
        builder.HasMany(x => x.FeedBacks)
            .WithOne(x => x.Participant)
            .HasForeignKey(x => x.ParticipantId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}