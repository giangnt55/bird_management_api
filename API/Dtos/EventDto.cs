﻿using AppCore.Models;
using MainData.Entities;

namespace API.Dtos;
public class EventDto : BaseDto
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
    public virtual IEnumerable<Participant>? Participants { get; set; }
    public virtual IEnumerable<FeedBack>? FeedBacks { get; set; }
}
public class EventCreateDto
{
    public string EventName { get; set; } = string.Empty;
    public EventStatus Status { get; set; }
    public EventType Type { get; set; }
    public string? CoverImage { get; set; }
    public string Description { get; set; } = string.Empty;
    public int MaxParticipants { get; set; }
    public int MinParticipants { get; set; }
    public DateTime StartDate { get; set; } = DateTime.Now;
    public DateTime EndDate { get; set; }
    public string Location { get; set; } = string.Empty;
    public string Prerequisite { get; set; } = string.Empty;
    public string EvaluationStrategy { get; set; } = string.Empty;
}

public class EventUpdateDto
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
}
public class EventDetailDto : BaseDto
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
    public virtual IEnumerable<Participant>? Participants { get; set; }
    public virtual IEnumerable<FeedBack>? FeedBacks { get; set; }
}
public class EventQueryDto : BaseQueryDto
{
    public string EventName { get; set; } = string.Empty;
}