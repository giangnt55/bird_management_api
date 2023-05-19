using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class ChatGroup : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    
    //Relationship
    public virtual IEnumerable<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();
    public virtual IEnumerable<ChatGroupMember> ChatGroupMembers { get; set; } = new List<ChatGroupMember>();
}

public class ChatGroupConfig : IEntityTypeConfiguration<ChatGroup>
{
    public void Configure(EntityTypeBuilder<ChatGroup> builder)
    {
        builder.Property(x => x.Name).IsRequired();
        builder.HasMany(x => x.ChatMessages).WithOne(x => x.ChatGroup)
            .HasForeignKey(x => x.ChatGroupId);
        builder.HasMany(x => x.ChatGroupMembers).WithOne(x => x.ChatGroup)
            .HasForeignKey(x => x.ChatGroupId);
    }
}