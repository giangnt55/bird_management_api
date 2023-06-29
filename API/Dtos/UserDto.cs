using AppCore.Models;
using MainData.Entities;

namespace API.Dtos;

public class UserDto : BaseDto
{
  public string? Fullname { get; set; }
  public string? Avatar { get; set; }
  public UserRole Role { get; set; }
  public  UserStatus Status { get; set; }
  public string? Email { get; set; }
  public string? PhoneNumber { get; set; }
  public string? Address { get; set; }
  public string? Username { get; set; }
}

public class FriendDto : BaseDto
{
  public string? Fullname { get; set; }
  public string? Avatar { get; set; }
  public UserRole Role { get; set; }
  public  UserStatus Status { get; set; }
  public string? Email { get; set; }
  public string? PhoneNumber { get; set; }
  public string? Address { get; set; }
  public string? Username { get; set; }
  public int TotalFollowing { get; set; }
  public int TotalFollower { get; set; }
  public int TotalPost { get; set; }
  public bool IsFollowedByLoggedInUser  { get; set; }
  public bool IsFollowingLoggedInUser   { get; set; }
}

public class UserQuery : BaseQueryDto
{
  public string? Keyword { get; set; }
}
