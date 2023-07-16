using AppCore.Data;
using MainData.Entities;

namespace MainData;

public class MainUnitOfWork : IDisposable
{
    private readonly DatabaseContext _context;

    public MainUnitOfWork(DatabaseContext context)
    {
        _context = context;
    }

    public BaseRepository<User> UserRepository => new(_context);
    public BaseRepository<Token> TokenRepository => new(_context);
    public BaseRepository<Notification> NotificationRepository => new(_context);
    public BaseRepository<Post> PostRepository => new(_context);
    //Not meet the deadline
    //public BaseRepository<News> NewsRepository => new(_context);
    public BaseRepository<Comment> CommentRepository => new(_context);
    public BaseRepository<Like> LikeRepository => new(_context);
    public BaseRepository<Bird> BirdRepository => new(_context);
    public BaseRepository<Report> ReportRepository => new(_context);
    public BaseRepository<Event> EventRepository => new(_context);
    public BaseRepository<Participant> ParticipantRepository => new(_context);
    public BaseRepository<FeedBack> FeedbackRepository => new(_context);
    public BaseRepository<Follower> FollowerRepository => new(_context);

    public void Dispose()
    {
    }
}
