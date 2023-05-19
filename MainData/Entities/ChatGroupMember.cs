using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class ChatGroupMember : BaseEntity
{
    public Guid ChatGroupId { get; set; }
    public Guid UserId { get; set; }
    public ChatGroupMemberRole Role { get; set; }
    
    //Relationship
    public virtual User User { get; set; } = new User();
    public virtual ChatGroup ChatGroup { get; set; } = new ChatGroup();
}

public enum ChatGroupMemberRole
{
    Admin = 1, Member  = 2
}

public class ChatGroupMemberConfig : IEntityTypeConfiguration<ChatGroupMember>
{
    public void Configure(EntityTypeBuilder<ChatGroupMember> builder)
    {
        builder.Property(x => x.ChatGroupId).IsRequired();
        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.Role).IsRequired();
        builder.HasOne(x => x.User);
        builder.HasOne(x => x.ChatGroup);
    }
}