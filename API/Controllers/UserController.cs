﻿using API.Dtos;
using API.Services;
using AppCore.Extensions;
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

  [HttpGet]
  [SwaggerOperation("Get users")]
  public async Task<ApiResponses<UserDto>> GetUsers([FromQuery]UserQuery userQuery)
  {
    return await _userService.Gets(userQuery);
  }

  [HttpGet("{id:guid}")]
  [SwaggerOperation("Get user detail")]
  public async Task<ApiResponse<UserDto>> GetUser(Guid id)
  {
    return await _userService.GetUserDetail(id);
  }


  [HttpPut("{id:guid}")]
  [SwaggerOperation("Update user information")]
  public async Task<ApiResponse> UpdateUser(Guid id, UserUpdate userUpdate)
  {
    return await _userService.UpdateInformation(id, userUpdate);
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
  
  // [HttpGet("Export")]
  // [SwaggerOperation("Get suggestion follow")]
  // public async Task<Stream> Export()
  // {
  //   var users = await _userService.ExportUser();
  //
  //   return ExportHelperList<UserDto>.Export(users, "Reporting", "Danh sách người dùng");
  // }
}
