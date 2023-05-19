using System.ComponentModel.DataAnnotations;
using MainData.Entities;

namespace API.Dtos;

public class AccountCredentialLoginDto
{
    [Required] public string Username { get; set; } = string.Empty;
    [Required] public string Password { get; set; } = string.Empty;
}

public class AuthDto
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime AccessExpiredAt { get; set; }
    public DateTime RefreshExpiredAt { get; set; }

    public string? Email { get; set; }
    public string? Fullname { get; set; }
    public UserRole Role { get; set; }
    public Guid UserId { get; set; }
    public bool IsFirstLogin { get; set; }
}