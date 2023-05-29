using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class Like : BaseEntity
{
    public Guid TargetId { get; set; }
    
    //Relationship
    public virtual Post? Post { get; set; }
    public virtual Comment? Comment { get; set; }
    public virtual User User { get; set; } = new User();
}

public class LikeConfig : IEntityTypeConfiguration<Like>
{
    public void Configure(EntityTypeBuilder<Like> builder)
    {
        builder.Property(x => x.TargetId).IsRequired();
        
        //Relationship
        builder.HasOne(x => x.User)
            .WithMany(x => x.Likes)
            .HasForeignKey(x => x.CreatorId);
            
        builder.HasOne(x => x.Comment)
            .WithMany(x => x.Likes)
            .HasForeignKey(x => x.TargetId)
            .OnDelete(DeleteBehavior.Restrict);;
        
        builder.HasOne(x => x.Post)
            .WithMany(x => x.Likes)
            .HasForeignKey(x => x.TargetId)
            .OnDelete(DeleteBehavior.Restrict);;
    }
}