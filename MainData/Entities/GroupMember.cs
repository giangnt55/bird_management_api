using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class GroupMember : BaseEntity
{
    public Guid GroupId { get; set; }
    public Guid UserId { get; set; }
    public MemberRole Role { get; set; }
    
    //Relationship
    public virtual User User { get; set; } = new User();
    public virtual Group Group { get; set; } = new Group();
}

public enum MemberRole
{
    Member = 1, Staff = 2
}

public class GroupMemberConfig : IEntityTypeConfiguration<GroupMember>
{
    public void Configure(EntityTypeBuilder<GroupMember> builder)
    {
        builder.Property(x => x.GroupId).IsRequired();
        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.Role).IsRequired();
        
        //Relationship
        builder.HasOne(x => x.User)
            .WithMany(x => x.GroupMembers)
            .HasForeignKey(x => x.UserId);
        
        builder.HasOne(x => x.Group)
            .WithMany(x => x.GroupMembers)
            .HasForeignKey(x => x.GroupId);
    }
}