using EngagementTracker.Core.Dtos;

namespace EngagementTracker.Core.Interfaces;

/// <summary>
/// Handles authentication: validates credentials and issues JWT tokens.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Authenticates a user with email and password.
    /// Returns a JWT access token and user profile on success.
    /// </summary>
    /// <param name="request">Login credentials.</param>
    /// <returns>JWT token and user profile.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when credentials are invalid.</exception>
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request);

    /// <summary>
    /// Returns the profile of the currently authenticated user.
    /// </summary>
    /// <param name="userId">The user ID from the JWT claim.</param>
    /// <returns>User profile data.</returns>
    Task<UserProfileDto> GetProfileAsync(int userId);
}
