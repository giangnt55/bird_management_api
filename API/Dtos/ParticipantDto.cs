using AppCore.Models;
using MainData.Entities;

namespace API.Dtos
{
    public class ParticipantDto : BaseDto
    {
        public Guid EventId { get; set; }
        public ParticipantRole Role { get; set; }

        //Relationship
        public virtual User User { get; set; } = new User();
        public virtual Event Event { get; set; } = new Event();
        public virtual IEnumerable<FeedBack>? FeedBacks { get; set; }
    }

    public class ParticipantCreateDto
    {
        public Guid EventId { get; set; }
        public ParticipantRole Role { get; set; }

    }
    public class ParticipantUpdateDto
    {
        public Guid EventId { get; set; }
        public ParticipantRole Role { get; set; }
    }
    public class DetailParticipantDto : BaseDto
    {
        public Guid EventId { get; set; }
        public ParticipantRole Role { get; set; }

        //Relationship
        public virtual User User { get; set; } = new User();
        public virtual Event Event { get; set; } = new Event();
    }
    public class ParticipantResultDto
    {
        public int TotalParticipants { get; set; }

    }

    public class ParticipantQueryDto : BaseQueryDto
    {
        
    }
}
