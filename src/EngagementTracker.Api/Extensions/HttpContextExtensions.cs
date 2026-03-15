using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using EngagementTracker.Core.Enums;

namespace EngagementTracker.Api.Extensions;

/// <summary>
/// Extension methods for extracting authenticated user information from HttpContext.
/// </summary>
public static class HttpContextExtensions
{
    /// <summary>
    /// Extracts the user ID and role from the JWT claims on the current request.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <returns>A tuple of (userId, userRole).</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when claims are missing or invalid.</exception>
    public static (int UserId, UserRole Role) GetUserClaims(this HttpContext context)
    {
        var userIdClaim = context.User.FindFirst(JwtRegisteredClaimNames.Sub)
            ?? context.User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            throw new UnauthorizedAccessException("Invalid or missing user ID in token.");
        }

        var roleClaim = context.User.FindFirst(ClaimTypes.Role);
        if (roleClaim == null || !Enum.TryParse<UserRole>(roleClaim.Value, out var role))
        {
            throw new UnauthorizedAccessException("Invalid or missing role in token.");
        }

        return (userId, role);
    }
}
