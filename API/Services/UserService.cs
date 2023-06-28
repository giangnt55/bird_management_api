using System.Linq.Expressions;
using API.Dtos;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;
using Microsoft.EntityFrameworkCore;

namespace API.Services;

public interface IUserService : IBaseService
{
  Task<ApiResponse<UserDto>> GetAccountInformation();
  Task<ApiResponses<FriendDto>> GetSuggestionFollow(UserQuery userQuery);
}

public class UserService : BaseService, IUserService
{
  public UserService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
  {
  }

  public async Task<ApiResponse<UserDto>> GetAccountInformation()
  {
    var account = await MainUnitOfWork.UserRepository.FindOneAsync<UserDto>(new Expression<Func<User, bool>>[]
    {
      x => !x.DeletedAt.HasValue,
      x => x.Id == AccountId
    });

    if (account == null)
      throw new ApiException("Not found user", StatusCode.NOT_FOUND);

    return ApiResponse<UserDto>.Success(account);
  }

  public async Task<ApiResponses<FriendDto>> GetSuggestionFollow(UserQuery userQuery)
  {

    var userDataSet = MainUnitOfWork.UserRepository.GetQuery().Where(x=> !x!.DeletedAt.HasValue);
    var followDataSet = MainUnitOfWork.FollowerRepository.GetQuery().Where(x=> !x!.DeletedAt.HasValue);

    var users = await userDataSet
      .Where(x => x.Id != AccountId)
      .GroupJoin(followDataSet,
        user => user.Id,
        follower => follower.FollowTo,
        (user, followers) => new
        {
          User = user,
          FollowerCount = followers.Count(),
          IsFollowedByLoggedInUser = followers.Any(f => f.CreatorId == AccountId),
          IsFollowingLoggedInUser = followers.Any(f => f.FollowTo == AccountId)
        })
      .OrderByDescending(x => x.FollowerCount)
      .Skip(userQuery.Skip())
      .Take(userQuery.PageSize)
      .Select(x => new FriendDto
      {
        Id = x.User!.Id,
        Address = x.User.Address,
        Avatar = x.User.Avatar,
        Username = x.User.Username,
        CreatorId = x.User.CreatorId ?? Guid.Empty,
        Email = x.User.Email,
        Role = x.User.Role,
        Fullname = x.User.Fullname,
        Status = x.User.Status,
        CreatedAt = x.User.CreatedAt,
        EditedAt = x.User.UpdatedAt,
        TotalFollow = x.FollowerCount,
        PhoneNumber = x.User.PhoneNumber,
        EditorId = x.User.EditorId ?? Guid.Empty,
        IsFollowedByLoggedInUser = x.IsFollowedByLoggedInUser,
        IsFollowingLoggedInUser = x.IsFollowingLoggedInUser
      })
      .ToListAsync();


    return ApiResponses<FriendDto>.Success(users);
  }
}

