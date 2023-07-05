using System.Linq.Expressions;
using API.Dtos;
using AppCore.Extensions;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;
using Microsoft.EntityFrameworkCore;

namespace API.Services;

public interface IUserService : IBaseService
{
  Task<ApiResponse<FriendDto>> GetUserInformation(string username);
  Task<ApiResponse<UserDto>> GetAccountInformation();
  Task<ApiResponses<FriendDto>> GetSuggestionFollow(UserQuery userQuery);
  Task<ApiResponses<UserDto>> Gets(UserQuery userQuery);
  Task<ApiResponse<UserDto>> GetUserDetail(Guid id);
  Task<ApiResponse> UpdateInformation(Guid id, UserUpdate userUpdate);
}

public class UserService : BaseService, IUserService
{
  public UserService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
  {
  }

  public async Task<ApiResponse<FriendDto>> GetUserInformation(string username)
  {
    var user = await MainUnitOfWork.UserRepository.FindOneAsync<FriendDto>(new Expression<Func<User, bool>>[]
    {
      x => !x.DeletedAt.HasValue,
      x => x.Username == username
    });

    if (user == null)
      throw new ApiException("Not existed username", StatusCode.BAD_REQUEST);

    var followDataSet = MainUnitOfWork.FollowerRepository.GetQuery().Where(x=> !x!.DeletedAt.HasValue);

    user.TotalFollower = followDataSet.Count(x => x!.FollowTo == user.Id);
    user.TotalFollowing = followDataSet.Count(x => x!.CreatorId == user.Id);

    user.IsFollowingLoggedInUser = followDataSet.Any(x => x.FollowTo == AccountId && x.CreatorId == user.Id);
    user.IsFollowedByLoggedInUser = followDataSet.Any(x => x.FollowTo == user.Id && x.CreatorId == AccountId);

    user.TotalPost = (await MainUnitOfWork.PostRepository.FindAsync(new Expression<Func<Post, bool>>[]
    {
      x => !x.DeletedAt.HasValue,
      x => x.CreatorId == user.Id
    }, null)).Count();

    return ApiResponse<FriendDto>.Success(user);
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
      .Where(x => x.IsFollowedByLoggedInUser == false)
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
        TotalFollower = x.FollowerCount,
        PhoneNumber = x.User.PhoneNumber,
        EditorId = x.User.EditorId ?? Guid.Empty,
        IsFollowedByLoggedInUser = x.IsFollowedByLoggedInUser,
        IsFollowingLoggedInUser = x.IsFollowingLoggedInUser
      })
      .ToListAsync();


    return ApiResponses<FriendDto>.Success(users);
  }

  public async Task<ApiResponses<UserDto>> Gets(UserQuery userQuery)
  {
    var users = await MainUnitOfWork.UserRepository.FindResultAsync<UserDto>(new Expression<Func<User, bool>>[]
    {
      x => !x.DeletedAt.HasValue,
    }, userQuery.OrderBy, userQuery.Skip(), userQuery.PageSize);

    users.Items = await _mapperRepository.MapCreator(users.Items.ToList());

    return ApiResponses<UserDto>.Success(
      users.Items,
      users.TotalCount,
      userQuery.PageSize,
      userQuery.Skip(),
      (int)Math.Ceiling(users.TotalCount / (double)userQuery.PageSize)
    );
  }

  public async Task<ApiResponse<UserDto>> GetUserDetail(Guid id)
  {
    var user = await MainUnitOfWork.UserRepository.FindOneAsync<UserDto>(new Expression<Func<User, bool>>[]
    {
      x => !x.DeletedAt.HasValue,
      x => x.Id == id
    });

    if (user == null)
      throw new ApiException("Not found this user", StatusCode.NOT_FOUND);

    user = await _mapperRepository.MapCreator(user);

    return ApiResponse<UserDto>.Success(user);
  }

  public async Task<ApiResponse> UpdateInformation(Guid id, UserUpdate userUpdate)
  {
    var user = await MainUnitOfWork.UserRepository.FindOneAsync<UserDto>(new Expression<Func<User, bool>>[]
    {
      x => !x.DeletedAt.HasValue,
      x => x.Id == id
    });

    if (user == null)
      throw new ApiException("Not found this user", StatusCode.NOT_FOUND);

    if (user.Id != AccountId)
    {
      var checkRole = await MainUnitOfWork.UserRepository.FindOneAsync(new Expression<Func<User, bool>>[]
      {
        x => !x.DeletedAt.HasValue,
        x => x.Id == AccountId,
        x => x.Role == UserRole.Admin || x.Role == UserRole.Staff
      });

      if (checkRole == null)
        throw new ApiException("Can't not update information of this user", StatusCode.BAD_REQUEST);

      if (userUpdate.Role != null)
      {
        user.Role = userUpdate.Role.Value;
      }
    }

    user.Fullname = userUpdate.Fullname ?? user.Fullname;
    user.Address = userUpdate.Address ?? user.Address;
    user.Status = userUpdate.Status ?? user.Status;
    user.Email = userUpdate.Email ?? user.Email;
    if (userUpdate.Status != null)
    {
      user.Status = userUpdate.Status.Value;
    }

    user.Avatar = userUpdate.Avatar ?? user.Avatar;
    user.PhoneNumber = userUpdate.PhoneNumber ?? user.PhoneNumber;

    return ApiResponse.Success();
  }

}

