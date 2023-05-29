using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class Group: BaseEntity
{
    public string GroupName { get; set; } = string.Empty;
    public string? CoverImage { get; set; }
    public string? Description { get; set; }
    
    //Relationship
    public virtual IEnumerable<GroupMember> GroupMembers { get; set; } = new List<GroupMember>();
    public virtual IEnumerable<Post> Posts { get; set; } = new List<Post>();
}

public class GroupConfig : IEntityTypeConfiguration<Group>
{
    public void Configure(EntityTypeBuilder<Group> builder)
    {
        builder.Property(x => x.GroupName).IsRequired();
        builder.Property(x => x.CoverImage).IsRequired(false);
        builder.Property(x => x.Description).IsRequired(false);
        
        //Relationship
        builder.HasMany(u => u.GroupMembers)
            .WithOne(x => x.Group)
            .HasForeignKey(t => t.GroupId);
        
        builder.HasMany(u => u.Posts)
            .WithOne(x => x.Group)
            .HasForeignKey(t => t.GroupId);
    }
}