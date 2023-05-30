using System.Linq.Expressions;
using API.Dtos;
using AppCore.Models;
using MainData;
using MainData.Entities;

namespace API.Services;

public interface IBirdService : IBaseService
{
    Task<ApiResponses<BirdDto>> GetBird(BirdQueryDto queryDto);
}

public class BirdService : BaseService, IBirdService
{
    public BirdService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor) : base(mainUnitOfWork, httpContextAccessor)
    {
    }


    public async Task<ApiResponses<BirdDto>> GetBird(BirdQueryDto queryDto)
    {
        var response = await MainUnitOfWork.BirdRepository.FindResultAsync<BirdDto>(new Expression<Func<Bird, bool>>[]
        {
            x => !x.DeletedAt.HasValue,
            x => string.IsNullOrEmpty(queryDto.BirdName) || x.Name.ToLower() == queryDto.BirdName.Trim().ToLower()
        }, queryDto.OrderBy, queryDto.Skip(), queryDto.PageSize);

        return ApiResponses<BirdDto>.Success(
            response.Items,
            response.TotalCount,
            queryDto.PageSize,
            queryDto.Skip(),
            (int)Math.Ceiling(response.TotalCount / (double)queryDto.PageSize)
        );
    }
}