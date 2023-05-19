using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class Post : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? Image { get; set; } = string.Empty;
    public int Like { get; set; }
    public Guid? ActivityId { get; set; }
    
    //Relationship
    public virtual User Creator { get; set; } = new User();
    public virtual Activity Activity { get; set; } = new Activity();
    public virtual IEnumerable<Interaction> Interactions { get; set; } = new List<Interaction>();
}

public class PostConfig : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.Property(x => x.Title).IsRequired();
        builder.Property(x => x.Content).IsRequired();
        builder.Property(x => x.Image).IsRequired();
        builder.Property(x => x.Like).IsRequired();
        builder.Property(x => x.ActivityId).IsRequired(false);
        builder.HasOne(x => x.Creator);
        builder.HasOne(x => x.Activity);
        builder.HasMany(x => x.Interactions).WithOne(x => x.Post)
            .HasForeignKey(x => x.PostId);
    }
}