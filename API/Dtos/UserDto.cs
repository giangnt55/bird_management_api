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
