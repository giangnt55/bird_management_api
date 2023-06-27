using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class Comment : BaseEntity
{
    public Guid? PostId { get; set; }
    public string Content { get; set; } = string.Empty;
    public Guid? ReplyTo { get; set; }

    //Relationship
    public virtual User? User { get; set; }
    public virtual Post? Post { get; set; }
    public virtual IEnumerable<Like>? Likes { get; set; }
    public virtual IEnumerable<Report>? Reports { get; set; }
}

public class CommentConfig : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.Property(x => x.Content).IsRequired();
        builder.Property(x => x.PostId).IsRequired(false);
        builder.Property(x => x.ReplyTo).IsRequired(false);

        //Relationship
        builder.HasOne(x => x.User)
            .WithMany(x => x.Comments)
            .HasForeignKey(x => x.CreatorId);

        builder.HasOne(x => x.Post)
            .WithMany(x => x.Comments)
            .HasForeignKey(x => x.PostId);

        builder.HasMany(x => x.Likes)
            .WithOne(x => x.Comment)
            .HasForeignKey(x => x.CommentId);

        builder.HasMany(x => x.Reports)
            .WithOne(x => x.Comment)
            .HasForeignKey(x => x.TargetId);
    }
}
