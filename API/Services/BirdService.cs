using System.Linq.Expressions;
using API.Dtos;
using AppCore.Models;
using MainData;
using MainData.Entities;

namespace API.Services;

public interface IBirdService : IBaseService
{
    Task<ApiResponses<BirdDto>> GetBird(BirdQueryDto queryDto);

    Task<ApiResponse<bool>> InsertBird(BirdCreateDto birdDto);
    Task<ApiResponse<bool>> UpdateBird(BirdUpdateDto birdDto);
    Task<ApiResponse<bool>> DeleteBird(BirdDeleteDto birdDto);

}

public class BirdService : BaseService, IBirdService
{
    public BirdService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor) : base(mainUnitOfWork, httpContextAccessor)
    {
    }

    public async Task<ApiResponse<bool>> DeleteBird(BirdDeleteDto birdDto)
    {
        var existingBird = await MainUnitOfWork.BirdRepository.FindOneAsync(birdDto.Id);
        if (existingBird == null)
        {
            return (ApiResponse<bool>)ApiResponse<bool>.Failed();
        }

        bool isDeleted = await MainUnitOfWork.BirdRepository.DeleteAsync(existingBird, AccountId);

        if (isDeleted)
        {
            return ApiResponse<bool>.Success(true);
        }
        else
        {
            return (ApiResponse<bool>)ApiResponse<bool>.Failed();
        }

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

    public async Task<ApiResponse<bool>> InsertBird(BirdCreateDto birdDto)
    {
        var bird = new Bird
        {
            Id = Guid.NewGuid(),
            Name = birdDto.Name,
            Image = birdDto.Image,
            Description = birdDto.Description,
            Habitat = birdDto.Habitat,
            AvgLifeSpan = birdDto.AvgLifeSpan,
            AvgLifeSize = birdDto.AvgLifeSize,
            Conservation = birdDto.Conservation,
        };
        bool response = await MainUnitOfWork.BirdRepository.InsertAsync(bird, AccountId);

        if (response)
        {
            return ApiResponse<bool>.Success(true);
        }
        else
        {
            return (ApiResponse<bool>)ApiResponse.Failed();
        }
    }

    public async Task<ApiResponse<bool>> UpdateBird(BirdUpdateDto birdDto)
    {
        var existingBird = await MainUnitOfWork.BirdRepository.FindOneAsync(birdDto.Id);

        if (existingBird == null)
        {
            return (ApiResponse<bool>)ApiResponse<bool>.Failed();
        }

        existingBird.Name = birdDto.Name;
        existingBird.Image = birdDto.Image;
        existingBird.Description = birdDto.Description;
        existingBird.Habitat = birdDto.Habitat;
        existingBird.AvgLifeSpan = birdDto.AvgLifeSpan;
        existingBird.AvgLifeSize = birdDto.AvgLifeSize;
        existingBird.Conservation = birdDto.Conservation;

        bool isUpdated = await MainUnitOfWork.BirdRepository.UpdateAsync(existingBird, AccountId);

        if (isUpdated)
        {
            return ApiResponse<bool>.Success(true);
        }
        else
        {
            return (ApiResponse<bool>)ApiResponse<bool>.Failed();
        }
    }
}