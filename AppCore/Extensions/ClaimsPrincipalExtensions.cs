using System.Security.Claims;
using AppCore.Models;

namespace AppCore.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid? GetId(this ClaimsPrincipal principal)
        {
            var value = principal.FindFirstValue(AppClaimTypes.Id);
            Guid.TryParse(value, out var accountId);
            return accountId == new Guid() ? null : accountId;
        }
        
        public static bool GetActive(this ClaimsPrincipal principal)
        {
            var value = principal.FindFirstValue(AppClaimTypes.Status);
            return bool.TryParse(value, out var isActive) && isActive;
        }

        public static string? GetRole(this ClaimsPrincipal principal)
        {
            return principal.FindFirstValue(AppClaimTypes.Role);
        }

        public static TEnum? GetEnum<TEnum>(this ClaimsPrincipal principal, string claimType) where TEnum : struct
        {
            var value = principal.FindFirstValue(claimType);
            return Enum.TryParse(value, out TEnum result) ? result : null;
        }
        
        public static string? Get(this ClaimsPrincipal principal, string type)
        {
            return principal.FindFirst(type)?.Value;
        }
    }
}