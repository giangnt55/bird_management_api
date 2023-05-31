using AppCore.Models;

namespace API.Dtos;

public class GroupDto : BaseDto
{
    public string GroupName { get; set; } = string.Empty;
    public string? CoverImage { get; set; }
    public string? Description { get; set; }
    
    public int TotalMember { get; set; }
}

public class GroupDetailDto : BaseDto
{
    public string GroupName { get; set; } = string.Empty;
    public string? CoverImage { get; set; }
    public string? Description { get; set; }
    
    public int TotalMember { get; set; }
}