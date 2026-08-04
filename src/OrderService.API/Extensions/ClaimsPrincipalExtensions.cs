using System.Security.Claims;

namespace OrderService.API.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        return Guid.Parse(
            user.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }

    public static string? GetEmail(this ClaimsPrincipal user)
    {
        return user.FindFirstValue(ClaimTypes.Email);
    }

    public static string? GetRole(this ClaimsPrincipal user)
    {
        return user.FindFirstValue(ClaimTypes.Role);
    }

    public static string? GetFullName(this ClaimsPrincipal user)
    {
        return user.FindFirstValue(ClaimTypes.Name);
    }
}