using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class ChatMessage : BaseEntity
{
    public Guid ChatGroupId { get; set; }
    public Guid SenderId { get; set; }
    public string Content { get; set; } = string.Empty;
    public ChatMessageStatus Status { get; set; }
    //Relationship
    public virtual ChatGroup ChatGroup { get; set; } = new ChatGroup();
    public virtual User Sender { get; set; } = new User();
}

public enum ChatMessageStatus
{
    Sending =1, Sent = 2, Seen = 3
}

public class ChatMessageConfig : IEntityTypeConfiguration<ChatMessage>
{
    public void Configure(EntityTypeBuilder<ChatMessage> builder)
    {
        builder.Property(x => x.ChatGroupId).IsRequired();
        builder.Property(x => x.SenderId).IsRequired();
        builder.Property(x => x.Content).IsRequired();
        builder.Property(x => x.Status).IsRequired();
        builder.HasOne(x => x.ChatGroup);
        builder.HasOne(x => x.Sender);
    }
}