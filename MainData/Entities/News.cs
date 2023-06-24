using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class News : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public NewsType Type { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? CoverImage { get; set; }
    public DateTime PublishDate { get; set; }
    public string Author { get; set; } = string.Empty;
}

public enum NewsType
{
    Health = 1, Business = 2, Entertainment = 3, Information = 4 
}

public class NewsConfig : IEntityTypeConfiguration<News>
{
    public void Configure(EntityTypeBuilder<News> builder)
    {
        builder.Property(x => x.Title).IsRequired();
        builder.Property(x => x.Type).IsRequired();
        builder.Property(x => x.Content).IsRequired();
        builder.Property(x => x.CoverImage).IsRequired(false);
        builder.Property(x => x.PublishDate).IsRequired();
        builder.Property(x => x.Author).IsRequired();
    }
}