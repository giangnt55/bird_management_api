using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class User : BaseEntity
{
    public string? Fullname { get; set; }
    public string? Avatar { get; set; }
    public UserRole Role { get; set; }
    public  UserStatus Status { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? Salt { get; set; }

    //Relationship
    public virtual IEnumerable<Token>? Tokens { get; set; }
    public virtual IEnumerable<Notification>? Notifications { get; set; }
    public virtual IEnumerable<Comment>? Comments { get; set; }
    public virtual IEnumerable<Like>? Likes { get; set; }
    public virtual IEnumerable<Report>? Reports { get; set; }
    public virtual IEnumerable<Participant>? Participants { get; set; }
    public virtual IEnumerable<Post>? Posts { get; set; }
    public virtual IEnumerable<Follower>? Followers { get; set; }
}

public enum UserRole
{
    Member = 1, Staff = 2, Manager = 3, Admin = 4
}

public enum UserStatus
{
    Active = 1, InActive = 2, Blocked = 3
}

public class UserConfig : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(x => x.Fullname).IsRequired(false);
        builder.Property(x => x.Avatar).IsRequired(false);
        builder.Property(x => x.Role).IsRequired();
        builder.Property(x => x.Email).IsRequired(false);
        builder.Property(x => x.PhoneNumber).IsRequired(false);
        builder.Property(x => x.Address).IsRequired(false);
        builder.Property(x => x.Username).IsRequired().HasMaxLength(50);;
        builder.Property(x => x.Password).IsRequired();
        builder.Property(x => x.Salt).IsRequired();

        //Relationship
        builder.HasMany(u => u.Tokens)
            .WithOne()
            .HasForeignKey(t => t.UserId);;

        builder.HasMany(u => u.Notifications)
            .WithOne(x => x.User)
            .HasForeignKey(t => t.UserId);

        builder.HasMany(u => u.Comments)
            .WithOne(x => x.User)
            .HasForeignKey(t => t.CreatorId);

        builder.HasMany(u => u.Likes)
            .WithOne(x => x.User)
            .HasForeignKey(t => t.CreatorId);

        builder.HasMany(u => u.Reports)
            .WithOne(x => x.User)
            .HasForeignKey(t => t.CreatorId);

        builder.HasMany(u => u.Participants)
            .WithOne(x => x.User)
            .HasForeignKey(t => t.CreatorId);

        builder.HasMany(u => u.Posts)
            .WithOne(x => x.User)
            .HasForeignKey(t => t.CreatorId);

        builder.HasMany(u => u.Followers)
          .WithOne(x => x.User)
          .HasForeignKey(t => t.FollowTo);

    }
}
