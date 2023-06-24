using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class Like : BaseEntity
{
    public Guid? PostId { get; set; }
    public Guid? CommentId { get; set; }
    
    //Relationship
    public virtual Post? Post { get; set; }
    public virtual Comment? Comment { get; set; }
    public virtual User? User { get; set; }
}

public class LikeConfig : IEntityTypeConfiguration<Like>
{
    public void Configure(EntityTypeBuilder<Like> builder)
    {
        builder.Property(x => x.PostId).IsRequired(false);
        builder.Property(x => x.CommentId).IsRequired(false);
        
        //Relationship
        builder.HasOne(x => x.User)
            .WithMany(x => x.Likes)
            .HasForeignKey(x => x.CreatorId);
            
        builder.HasOne(x => x.Comment)
            .WithMany(x => x.Likes)
            .HasForeignKey(x => x.CommentId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);;
        
        builder.HasOne(x => x.Post)
            .WithMany(x => x.Likes)
            .HasForeignKey(x => x.PostId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);
        
    }
}