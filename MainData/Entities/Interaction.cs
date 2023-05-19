using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class Interaction : BaseEntity
{
    public Guid PostId { get; set; }
    public InteractType Type { get; set; }
    public string? Content { get; set; }
    
    //Relationship
    public virtual Post Post { get; set; } = new Post();
    public virtual User User { get; set; } = new User();
}

public enum InteractType
{
    Like = 1, Comment = 2
}

public class InteractionConfig : IEntityTypeConfiguration<Interaction>
{
    public void Configure(EntityTypeBuilder<Interaction> builder)
    {
        builder.Property(x => x.PostId).IsRequired();
        builder.Property(x => x.Content).IsRequired(false);
        builder.Property(x => x.Type).IsRequired();
        builder.HasOne(x => x.User);
        builder.HasOne(x => x.Post);
    }
}