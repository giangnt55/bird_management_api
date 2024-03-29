﻿using AppCore.Models;
using MainData.Entities;

namespace API.Dtos;
public class EventDto : BaseDto
{
    public string EventName { get; set; } = string.Empty;
    public EventStatus Status { get; set; }
    public EventType Type { get; set; }
    public HostType HostType { get; set; }  
    public string? CoverImage { get; set; }
    public string Description { get; set; } = string.Empty;
    public int MaxParticipants { get; set; }
    public int MinParticipants { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Location { get; set; } = string.Empty;
    public string Prerequisite { get; set; } = string.Empty;
    public string EvaluationStrategy { get; set; } = string.Empty;
    public int TotalParticipant { get; set; }
    public int TotalFeedback { get; set; }
    public decimal AverageRating { get; set; }
    
    public bool IsJoined { get; set; }

}
public class EventCreateDto
{
  public string EventName { get; set; } = string.Empty;
  public EventStatus? Status { get; set; } = EventStatus.UpComing;
  public EventType? Type { get; set; } = EventType.Information;
  public HostType HostType { get; set; }
  public string? CoverImage { get; set; }
  public string Description { get; set; } = string.Empty;
  public int? MaxParticipants { get; set; }
  public int? MinParticipants { get; set; } = 0;
  public DateTime StartDate { get; set; }
  public DateTime? EndDate { get; set; }
  public string? Location { get; set; }
  public string? Prerequisite { get; set; }
  public string? EvaluationStrategy { get; set; }
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
  public HostType HostType { get; set; }
  public string? CoverImage { get; set; }
  public string Description { get; set; } = string.Empty;
  public int MaxParticipants { get; set; }
  public int MinParticipants { get; set; }
  public DateTime StartDate { get; set; }
  public DateTime? EndDate { get; set; }
  public string? Location { get; set; }
  public string? Prerequisite { get; set; }
  public string? EvaluationStrategy { get; set; }
  public bool IsJoined { get; set; }
  public int TotalParticipant { get; set; }
}
public class EventQueryDto : BaseQueryDto
{
    public string? Keyword { get; set; }
}
