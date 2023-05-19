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
    public virtual IEnumerable<Token> Tokens { get; set; } = new List<Token>();
    public virtual IEnumerable<Notification> Notifications { get; set; } = new List<Notification>();
    public virtual IEnumerable<Article> Articles { get; set; } = new List<Article>();
    public virtual IEnumerable<Activity> Activities { get; set; } = new List<Activity>();
    public virtual IEnumerable<ActivityFeedBack> ActivityFeedBacks { get; set; } = new List<ActivityFeedBack>();
    public virtual IEnumerable<Attendance> Attendances { get; set; } = new List<Attendance>();
    public virtual IEnumerable<Post> Posts { get; set; } = new List<Post>();
    public virtual IEnumerable<Interaction> Interactions { get; set; } = new List<Interaction>();
}

public enum UserRole
{
    Member = 1, Guest = 2, Admin = 3
}

public enum UserStatus
{
    Active = 1, InActive = 2 
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
        builder.HasMany(u => u.Tokens);
        builder.HasMany(u => u.Notifications).WithOne(x => x.User)
            .HasForeignKey(x => x.UserId);
        builder.HasMany(u => u.Articles).WithOne(x => x.Creator)
            .HasForeignKey(x => x.CreatorId);
        builder.HasMany(u => u.Activities).WithOne(x => x.Host)
            .HasForeignKey(x => x.CreatorId);
        builder.HasMany(u => u.ActivityFeedBacks).WithOne(x => x.User)
            .HasForeignKey(x => x.CreatorId);
        builder.HasMany(u => u.Attendances).WithOne(x => x.User)
            .HasForeignKey(x => x.UserId);
        builder.HasMany(u => u.Posts).WithOne(x => x.Creator)
            .HasForeignKey(x => x.CreatorId);
        builder.HasMany(u => u.Interactions).WithOne(x => x.User)
            .HasForeignKey(x => x.CreatorId);
    }
}