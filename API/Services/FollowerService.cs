using API.Dtos;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using AppCore.Extensions;

namespace API.Services
{
    public interface IFollowerService : IBaseService
    {
        public Task<ApiResponse> CreateFollower(FollowerCreateDto followerCreate);
        public Task<ApiResponse> UnFollower(Guid id);
        public Task<ApiResponses<FollowerDto>> GetFollowerOfUser(FollowerQuery queryDto);
        public Task<ApiResponses<FollowToDto>> GetFollowToOfUser(FollowerQuery queryDto);
    }
    public class FollowerService : BaseService, IFollowerService
    {
        public FollowerService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
        {
        }


        public async Task<ApiResponse> CreateFollower(FollowerCreateDto followerCreate)
        {
            var existingUser = await MainUnitOfWork.UserRepository.GetQuery()
    .FirstOrDefaultAsync(x => x.Id == followerCreate.FollowTo);

            if (existingUser == null)
            {
                // User does not exist
                throw new ApiException("User does not exist", StatusCode.NOT_FOUND);
            }
            // User exists
            var existingFollower = await MainUnitOfWork.FollowerRepository.GetQuery()
                .FirstOrDefaultAsync(x => x.FollowTo == followerCreate.FollowTo && x.CreatorId == AccountId);

            if (existingFollower != null && existingFollower.EditorId != null)
            {
                await MainUnitOfWork.FollowerRepository.UpdateEditorAsync(existingFollower, null, CurrentDate);
                return ApiResponse.Success();

            }
            // Follower already exists
            if (existingFollower!=null)
                throw new ApiException("Follower already exists", StatusCode.ALREADY_EXISTS);
            // Create a new follower
            var follower = new Follower
            {
                FollowTo = followerCreate.FollowTo
            };

            if (!await MainUnitOfWork.FollowerRepository.InsertAsync(follower, AccountId, CurrentDate))
            {
                // Failed to create follower
                throw new ApiException("Failed to create follower", StatusCode.SERVER_ERROR);
            }
            return ApiResponse.Success();
        }

        public async Task<ApiResponses<FollowerDto>> GetFollowerOfUser(FollowerQuery queryDto)
        {
          var followers = await MainUnitOfWork.FollowerRepository.FindAsync<FollowerDto>(
            new Expression<Func<Follower, bool>>[]
            {
              x => !x.DeletedAt.HasValue,
              x => (queryDto.FollowTo != null && x.FollowTo == queryDto.FollowTo) || (queryDto.FollowTo == null && AccountId == x.FollowTo),
            },
            null
          );

            var followersDataset = MainUnitOfWork.FollowerRepository.GetQuery();

            followers = await _mapperRepository.MapCreator(followers);

            foreach (var follow in followers)
            {
              if (queryDto.FollowTo != null)
              {
                var isFollowed = await followersDataset.AnyAsync(x =>
                  x!.CreatorId == queryDto.FollowTo && x.FollowTo == follow.FollowTo);

                follow.IsFollowed = isFollowed;
              }
              else
              {
                var isFollowed = await followersDataset.AnyAsync(x =>
                  x!.CreatorId == AccountId && x.FollowTo == follow.FollowTo);

                follow.IsFollowed = isFollowed;
              }
            }

            return ApiResponses<FollowerDto>.Success(followers);
        }

        public async Task<ApiResponses<FollowToDto>> GetFollowToOfUser(FollowerQuery queryDto)
        {
          var followers = await MainUnitOfWork.FollowerRepository.FindAsync<FollowToDto>(
            new Expression<Func<Follower, bool>>[]
            {
              x => !x.DeletedAt.HasValue,
              x => (queryDto.CreatorId != null && x.CreatorId == queryDto.CreatorId) || (queryDto.CreatorId == null && x.CreatorId == AccountId),
            },
            null
          );


          var userDataSet = MainUnitOfWork.UserRepository.GetQuery().Where(x => !x!.DeletedAt.HasValue);

          followers = await _mapperRepository.MapCreator(followers);

          foreach (var follow in followers)
          {
            follow.FollowToUser =
              (await userDataSet.FirstOrDefaultAsync(x => x!.Id == follow.FollowTo))!.ProjectTo<User, UserDto>();
          }

          return ApiResponses<FollowToDto>.Success(followers);
        }

        public async Task<ApiResponse> UnFollower(Guid id)
        {
            var existingFollower = await MainUnitOfWork.FollowerRepository.FindOneAsync(new Expression<Func<Follower, bool>>[]
            {
              x => !x.DeletedAt.HasValue,
              x => x.Id == id
            });
            if (existingFollower == null)
                throw new ApiException("Not found this follow", StatusCode.NOT_FOUND);
            if (existingFollower.FollowTo == AccountId && existingFollower.Id  == id)
                throw new ApiException("Can't not delete other's followers", StatusCode.BAD_REQUEST);
            if (!await MainUnitOfWork.FollowerRepository.UpdateEditorAsync(existingFollower, AccountId, CurrentDate))
                throw new ApiException("Can't not delete", StatusCode.SERVER_ERROR);

            return ApiResponse.Success();
        }
    }
}
