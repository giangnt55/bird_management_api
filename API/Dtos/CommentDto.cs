using AppCore.Models;

namespace API.Dtos;

public class CommentDto : BaseDto
{
    public Guid PostId { get; set; }
    public string Content { get; set; } = string.Empty;
    public Guid? ReplyTo { get; set; }
    public int TotalLike { get; set; }
    public int TotalReply { get; set; }
    public List<CommentDto> Replies { get; set; } = new List<CommentDto>();
}

public class DetailCommentDto : BaseDto
{
    public Guid PostId { get; set; }
    public string Content { get; set; } = string.Empty;
    public Guid? ReplyTo { get; set; }
    public int TotalLike { get; set; }
    public int TotalReply { get; set; }
    public List<CommentDto> Replies { get; set; } = new List<CommentDto>();
}

public class CommentCreateDto
{
  public Guid? PostId { get; set; }
  public string Content { get; set; } = string.Empty;
  public Guid? ReplyTo { get; set; }
}

public class TotalCommentDto
{
    public int TotalComemnts { get; set; }
}
