using System.ComponentModel.DataAnnotations;
using AppCore.Models;
using MainData.Entities;

namespace API.Dtos;

public class UserDto : BaseDto
{
  [Display(Name = "Fullname", Order = 3)] 
  public string? Fullname { get; set; }
  [Display(Name = "Introduction", Order = 4)] 
  public string? Introduction { get; set; }
  [Display(Name = "Avatar", Order = 5)] 
  public string? Avatar { get; set; }
  [Display(Name = "Role", Order = 6)] 
  public UserRole Role { get; set; }
  [Display(Name = "Status", Order = 7)] 
  public  UserStatus Status { get; set; }
  [Display(Name = "Email", Order = 8)] 
  public string? Email { get; set; }
  [Display(Name = "PhoneNumber", Order = 9)] 
  public string? PhoneNumber { get; set; }
  [Display(Name = "Address", Order = 10)] 
  public string? Address { get; set; }
  [Display(Name = "Username", Order = 11)] 
  public string? Username { get; set; }
}

public class FriendDto : BaseDto
{
  public string? Fullname { get; set; }
  public string? Introduction { get; set; }
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

public class UserUpdate
{
  public string? Fullname { get; set; }
  public string? Introduction { get; set; }
  public string? Avatar { get; set; }
  public UserRole? Role { get; set; }
  public  UserStatus? Status { get; set; }
  public string? Email { get; set; }
  public string? PhoneNumber { get; set; }
  public string? Address { get; set; }
}
