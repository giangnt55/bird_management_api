using AppCore.Extensions;
using MainData;

namespace API.Services;

public interface IBaseService
{
}

public class BaseService : IBaseService
{
    internal DateTime CurrentDate = DatetimeExtension.UtcNow();
    internal readonly MainUnitOfWork MainUnitOfWork;
    internal readonly IHttpContextAccessor HttpContextAccessor;
    internal Guid? AccountId;
    public BaseService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor)
    {
        MainUnitOfWork = mainUnitOfWork;
        HttpContextAccessor = httpContextAccessor;
        AccountId = httpContextAccessor.HttpContext?.User.GetId();
    }
}