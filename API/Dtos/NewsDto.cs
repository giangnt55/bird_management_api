using AppCore.Models;
using MainData.Entities;

namespace API.Dtos
{
    public class NewsDto : BaseDto
    {
        public string Title { get; set; } = string.Empty;
        public NewsType Type { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? CoverImage { get; set; }
        public DateTime PublishDate { get; set; }
        public string Author { get; set; } = string.Empty;
    }

    public class NewsCreateDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public NewsType Type { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? CoverImage { get; set; }
        public DateTime PublishDate { get; set; }
        public string Author { get; set; } = string.Empty;
    }

    public class NewsUpdateDto
    {
        public string Title { get; set; } = string.Empty;
        public NewsType Type { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? CoverImage { get; set; }
        public DateTime PublishDate { get; set; }
        public string Author { get; set; } = string.Empty;
    }

    public class GetNewsDto
    {
        public Guid Id { get; set; }
    }

    public class NewsQueryDto : BaseQueryDto
    {
        public string? Title { get; set; }
    }


}
