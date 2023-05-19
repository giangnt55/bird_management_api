using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class Article : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? CoverImage { get; set; } = string.Empty;
    public ArticleType Type { get; set; }
    
    //Relationship
    public virtual User Creator { get; set; } = new User();
}

public enum ArticleType
{
    News = 1, Blog = 2
}

public class ArticleConfig : IEntityTypeConfiguration<Article>
{
    public void Configure(EntityTypeBuilder<Article> builder)
    {
        builder.Property(x => x.Title).IsRequired();
        builder.Property(x => x.Content).IsRequired();
        builder.Property(x => x.Type).IsRequired();
        builder.Property(x => x.CoverImage).IsRequired(false);
        builder.HasOne(x => x.Creator);
    }
}