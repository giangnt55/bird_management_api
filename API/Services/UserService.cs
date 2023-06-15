using System.Linq.Expressions;
using API.Dtos;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;

namespace API.Services;

public interface IUserService : IBaseService
{
  Task<ApiResponse<UserDto>> GetAccountInformation();
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
      x => x.DeletedAt.HasValue,
      x => x.Id == AccountId
    });

    if (account == null)
      throw new ApiException("Not found user", StatusCode.NOT_FOUND);

    return ApiResponse<UserDto>.Success(account);
  }
}
