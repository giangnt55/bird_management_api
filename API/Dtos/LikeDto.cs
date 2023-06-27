using API.Controllers;
using AppCore.Models;

namespace API.Dtos;

public class LikeDto : BaseDto
{
}

public class CreateLikeDto
{
    public Guid? PostId { get; set; }
    public Guid? CommentId { get; set; }
}

public class LikeResultDto
{
    public int TotalLikes { get; set; }
}
