using AppCore.Models;
using MainData.Entities;

namespace API.Dtos;
public class FeedbackDto : BaseDto
{
    public Guid ParticipantId { get; set; }
    public Guid EventId { get; set; }
    public string? Content { get; set; }
    public RateStar? Rating { get; set; }


    //Relationship
    public virtual Event? Event { get; set; }
    public virtual Participant? Participant { get; set; }
}

public class FeedbackCreateDto
{
    public Guid ParticipantId { get; set; }
    public Guid EventId { get; set; }
    public string? Content { get; set; }
    public RateStar? Rating { get; set; }
    public float? AverageRating { get; set; }
}

public class FeedbackUpdateDto
{
    public Guid ParticipantId { get; set; }
    public Guid EventId { get; set; }
    public string? Content { get; set; }
    public RateStar? Rating { get; set; }
}

public class FeedbackDetailDto : BaseDto
{
    public Guid ParticipantId { get; set; }
    public Guid EventId { get; set; }
    public string? Content { get; set; }
    public RateStar? Rating { get; set; }
}

public class FeedbackQueryDto : BaseQueryDto
{
    public string? Content { get; set; }
    public Guid EventId { get; set; }

}