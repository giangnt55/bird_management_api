using AppCore.Models;

namespace API.Dtos;

public class LikePostDto
{
    public Guid PostId { get; set; }
}

public class LikeCommentDto
{
    //public Guid PostId { get; set; }
    public Guid CommentId { get; set; }
}

public class LikeResultDto
{
    public int TotalLikes { get; set; }
}