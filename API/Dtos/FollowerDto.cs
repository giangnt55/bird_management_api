using AppCore.Models;
using MainData.Entities;

namespace API.Dtos
{
    public class FollowerDto : BaseDto
    {
        public Guid FollowTo { get; set; }  

        //Relationship
        public virtual User? User { get; set; }
    }
    public class FollowerCreateDto
    {
        public Guid FollowTo { get; set; }
    }

    public class FollowerQuery : BaseQueryDto
    {
        public Guid FollowTo { get;}
    }
    public class  FollowerDetailDto : BaseDto
    {
        public bool isFollow { get; set; }
    }
}
