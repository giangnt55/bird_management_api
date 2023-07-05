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
    public DbSet<Post> Post { set; get; }
    public DbSet<News> News { set; get; }
    public DbSet<Bird> Birds { set; get; }
    public DbSet<Comment> Comments { set; get; }
    public DbSet<Like> Likes { set; get; }
    public DbSet<Notification> Notifications { set; get; }
    public DbSet<Event> Events { set; get; }
    public DbSet<Participant> Participants { set; get; }
    public DbSet<FeedBack> FeedBacks { set; get; }
    public DbSet<Report> Reports { set; get; }
    public DbSet<Follower> Followers { set; get; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfig());
        modelBuilder.ApplyConfiguration(new TokenConfig());
        modelBuilder.ApplyConfiguration(new PostConfig());
        modelBuilder.ApplyConfiguration(new NewsConfig());
        modelBuilder.ApplyConfiguration(new BirdConfig());
        modelBuilder.ApplyConfiguration(new CommentConfig());
        modelBuilder.ApplyConfiguration(new LikeConfig());
        modelBuilder.ApplyConfiguration(new NotificationConfig());
        modelBuilder.ApplyConfiguration(new EventConfig());
        modelBuilder.ApplyConfiguration(new ParticipantConfig());
        modelBuilder.ApplyConfiguration(new FeedBackConfig());
        modelBuilder.ApplyConfiguration(new ReportConfig());
        modelBuilder.ApplyConfiguration(new FollowerConfig());
    }
}
