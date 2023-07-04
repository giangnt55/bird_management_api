using AppCore.Models;
using MainData.Entities;

namespace API.Dtos
{
    public class FollowerDto : BaseDto
    {
        public Guid FollowTo { get; set; }
        public bool IsFollowed { get; set; }
    }

    public class FollowToDto : BaseDto
    {
      public Guid FollowTo { get; set; }
      public UserDto? FollowToUser { get; set; }
    }

    public class FollowerCreateDto
    {
        public Guid FollowTo { get; set; }
    }

    public class FollowerQuery : BaseQueryDto
    {
        public Guid? FollowTo { get; set; }
        public Guid? CreatorId { get; set; }
    }

}
