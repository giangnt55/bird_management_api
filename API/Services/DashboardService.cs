using System.Linq.Expressions;
using API.Dtos;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;

namespace API.Services;

public interface IDashboardService: IBaseService
{
    public Task<ApiResponse<DashboardDto>> GetBaseInformation();
}

public class DashboardService: BaseService, IDashboardService
{
    public DashboardService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
    {
    }

    public async Task<ApiResponse<DashboardDto>> GetBaseInformation()
    {
        var member = (await MainUnitOfWork.UserRepository.FindAsync(new Expression<Func<User, bool>>[]
        {
            x => !x.DeletedAt.HasValue,
            x => x.Role != UserRole.Admin
        },null)).Count;
        
        var newMember =  (await MainUnitOfWork.UserRepository.FindAsync(new Expression<Func<User, bool>>[]
        {
            x => !x.DeletedAt.HasValue,
            x => x.Role != UserRole.Admin,
            x => x.CreatedAt.Month == DateTime.Now.Month && x.CreatedAt.Year == DateTime.Now.Year
        },null)).Count;
        
        var totalEvent = (await MainUnitOfWork.EventRepository.FindAsync(new Expression<Func<Event, bool>>[]
        {
            x => !x.DeletedAt.HasValue
        },null)).Count;
        
        var totalPost = (await MainUnitOfWork.PostRepository.FindAsync(new Expression<Func<Post, bool>>[]
        {
            x => !x.DeletedAt.HasValue
        },null)).Count;

        return ApiResponse<DashboardDto>.Success(new DashboardDto
        {
            TotalEvent = totalEvent,
            TotalMember = member,
            TotalPost = totalPost,
            TotalNewMember = newMember
        });
    }
}