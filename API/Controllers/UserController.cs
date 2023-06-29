using API.Dtos;
using API.Services;
using AppCore.Models;
using MainData.Entities;
using MainData.Middlewares;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers;

public class UserController: BaseController
{
  private readonly IUserService _userService;

  public UserController(IUserService userService)
  {
    _userService = userService;
  }

  [HttpGet("account-infor")]
  [SwaggerOperation("Get current account information")]
  public async Task<ApiResponse<UserDto>> GetAccountInformation()
  {
    return await _userService.GetAccountInformation();
  }

  [HttpGet("{username}")]
  [SwaggerOperation("Get account information by username")]
  public async Task<ApiResponse<FriendDto>> GetUserInformation(string username)
  {
    return await _userService.GetUserInformation(username);
  }

  [HttpGet("suggestion")]
  [SwaggerOperation("Get suggestion follow")]
  public async Task<ApiResponses<FriendDto>> GetSuggestionFollow([FromQuery]UserQuery userQuery)
  {
    return await _userService.GetSuggestionFollow(userQuery);
  }
}
