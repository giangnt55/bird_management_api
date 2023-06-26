using AppCore.Models;

namespace API.Dtos;

public class LikeDto
{
    public Guid PostId { get; set; }
}

public class LikeResultDto
{
    public int TotalLikes { get; set; }
}