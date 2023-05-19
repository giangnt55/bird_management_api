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
    public BaseRepository<Species> SpeciesRepository => new(_context);
    public BaseRepository<Bird> BirdRepository => new(_context);
    public BaseRepository<Notification> NotificationRepository => new(_context);
    public BaseRepository<Article> NewsRepository => new(_context);
    public BaseRepository<ChatGroup> ChatGroupRepository => new(_context);
    public BaseRepository<ChatGroupMember> ChatGroupMemberRepository => new(_context);
    public BaseRepository<ChatMessage> ChatMessageRepository => new(_context);
    public BaseRepository<Activity> ActivityRepository => new(_context);
    public BaseRepository<ActivityFeedBack> ActivityFeedBackRepository => new(_context);
    public BaseRepository<Post> PostRepository => new(_context);
    public BaseRepository<Interaction> InteractionRepository => new(_context);
    public BaseRepository<Attendance> AttendanceRepository => new(_context);

    public void Dispose()
    {
    }
}