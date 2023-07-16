using AppCore.Models;
using MainData.Entities;

namespace API.Dtos;

public class ReportDto : BaseDto
{
    
}

public class ReportCreateDto
{
    public Guid? CommentId { get; set; }
    public Guid? PostId { get; set; }
    
    public ReportType Type { get; set; } 
    
    //public ReportType Type { get; set; } 
}

public enum ReportFor
{
    Post = 1, Comment = 2
}