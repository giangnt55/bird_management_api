using System.Security.Cryptography.X509Certificates;
using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class Post : BaseEntity
{
    public string? Tittle { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? Image { get; set; }
    
    //Relationship
    public virtual IEnumerable<Comment>? Comments { get; set; }
    public virtual IEnumerable<Like>? Likes { get; set; }
    public virtual IEnumerable<Report>? Reports { get; set; }
    public virtual User? User { get; set; }
}

public class PostConfig : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.Property(x => x.Image).IsRequired(false);
        builder.Property(x => x.Tittle).IsRequired(false);
        builder.Property(x => x.Content).IsRequired();
        
        //Relationship
        
        builder.HasMany(x => x.Comments)
            .WithOne(x => x.Post)
            .HasForeignKey(x => x.PostId);
        
        builder.HasMany(x => x.Likes)
            .WithOne(x => x.Post)
            .HasForeignKey(x => x.TargetId);
        
        builder.HasMany(x => x.Reports)
            .WithOne(x => x.Post)
            .HasForeignKey(x => x.TargetId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.User)
            .WithMany(x => x.Posts)
            .HasForeignKey(x => x.CreatorId);
    }
}