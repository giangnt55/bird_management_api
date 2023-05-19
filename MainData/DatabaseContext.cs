using MainData.Entities;
using Microsoft.EntityFrameworkCore;

namespace MainData;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }
    
    public DbSet<User> Users { set; get; }
    public DbSet<Token> Tokens { set; get; }
    public DbSet<Species> Species { set; get; }
    public DbSet<Bird> Birds { set; get; }
    public DbSet<Notification> Notifications { set; get; }
    public DbSet<Article> Articles { set; get; }
    public DbSet<ChatGroup> ChatGroups { set; get; }
    public DbSet<ChatGroupMember> ChatGroupMembers { set; get; }
    public DbSet<ChatMessage> ChatMessages { set; get; }
    public DbSet<Activity> Activities { set; get; }
    public DbSet<ActivityFeedBack> ActivityFeedBacks { set; get; }
    public DbSet<Attendance> Attendances { set; get; }
    public DbSet<Interaction> Interactions { set; get; }
    public DbSet<Post> Posts { set; get; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfig());
        modelBuilder.ApplyConfiguration(new TokenConfig());
        modelBuilder.ApplyConfiguration(new SpeciesConfig());
        modelBuilder.ApplyConfiguration(new BirdConfig());
        modelBuilder.ApplyConfiguration(new NotificationConfig());
        modelBuilder.ApplyConfiguration(new ArticleConfig());
        modelBuilder.ApplyConfiguration(new ChatGroupConfig());
        modelBuilder.ApplyConfiguration(new ChatMessageConfig());
        modelBuilder.ApplyConfiguration(new ChatGroupMemberConfig());
        modelBuilder.ApplyConfiguration(new ActivityConfig());
        modelBuilder.ApplyConfiguration(new ActivityFeedBackConfig());
        modelBuilder.ApplyConfiguration(new AttendanceConfig());
        modelBuilder.ApplyConfiguration(new InteractionConfig());
        modelBuilder.ApplyConfiguration(new PostConfig());
    }
}