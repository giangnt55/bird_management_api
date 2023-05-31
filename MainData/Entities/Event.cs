using System.Security.Cryptography.X509Certificates;
using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class Event : BaseEntity
{
    public string EventName { get; set; } = string.Empty;
    public EventStatus Status { get; set; }
    public EventType Type { get; set; }
    public string? CoverImage { get; set; }
    public string Description { get; set; } = string.Empty;
    public int MaxParticipants { get; set; }
    public int MinParticipants { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Location { get; set; } = string.Empty;
    public string Prerequisite { get; set; } = string.Empty;
    public string EvaluationStrategy { get; set; } = string.Empty;
    
    //Relationship
    public virtual IEnumerable<Participant> Participants { get; set; } = new List<Participant>();
    public virtual IEnumerable<FeedBack> FeedBacks { get; set; } = new List<FeedBack>();
}

public enum EventType
{
    Competition = 1, Entertainment = 2
}

public enum EventStatus
{
    UpComing = 1,
    Happening = 2,
    Cancelled = 3,
    Ending = 4
}

public class EventConfig : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.Property(x => x.EventName).IsRequired();
        builder.Property(x => x.Status).IsRequired();
        builder.Property(x => x.Type).IsRequired();
        builder.Property(x => x.CoverImage).IsRequired(false);
        builder.Property(x => x.Description).IsRequired();
        builder.Property(x => x.MinParticipants).IsRequired();
        builder.Property(x => x.MaxParticipants).IsRequired();
        builder.Property(x => x.StartDate).IsRequired();
        builder.Property(x => x.EndDate).IsRequired();
        builder.Property(x => x.Location).IsRequired();
        builder.Property(x => x.Prerequisite).IsRequired();
        builder.Property(x => x.EvaluationStrategy).IsRequired();
        
        //Relationship
        builder.HasMany(x => x.Participants)
            .WithOne(x => x.Event)
            .HasForeignKey(x => x.EventId);
        
        builder.HasMany(x => x.FeedBacks)
            .WithOne(x => x.Event)
            .HasForeignKey(x => x.EventId);

    }
}