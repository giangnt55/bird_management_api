using AppCore.Models;
using MainData.Entities;

namespace API.Dtos
{
    public class PostCreateDto : BaseDto
    {
        public string? Tittle { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? Image { get; set; }
    }

    public class PostDeleteDto
    {
        public Guid Id { get; set; }
    }
}
