using AppCore.Models;
using Azure.Core.Pipeline;
using MainData.Entities;

namespace API.Dtos;

public class PostCreateDto
{
    public string? Tittle { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? Image { get; set; }
}

public class PostUpdateDto
{
    public string? Tittle { get; set; }
    public string? Content { get; set; }
    public string? Image { get; set; }
}

public class PostDto : BaseDto
{
    public string? Tittle { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? Image { get; set; }
    public bool IsLiked { get; set; }
    public int TotalLike { get; set; }
    public int TotalComment { get; set; }
}

public class DetailPostDto : BaseDto
{
    public string? Tittle { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? Image { get; set; }
    public int TotalLike { get; set; }
    public int TotalComment { get; set; }
    public List<DetailCommentDto> Comments { get; set; } = new List<DetailCommentDto>();
}

public class PostQueryDto : BaseQueryDto
{
    public string? Keyword { get; set; }
}
