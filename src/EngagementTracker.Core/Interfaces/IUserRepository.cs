using EngagementTracker.Core.Enums;

namespace EngagementTracker.Core.Interfaces;

/// <summary>
/// Repository abstraction for user data access.
/// The Core layer defines this interface; Infrastructure implements it.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Finds a user by their email address. Returns null if not found.
    /// </summary>
    Task<UserRecord?> GetByEmailAsync(string email);

    /// <summary>
    /// Finds a user by their ID. Returns null if not found.
    /// </summary>
    Task<UserRecord?> GetByIdAsync(int id);
}

/// <summary>
/// Lightweight user record returned by the repository.
/// Decouples Core services from Infrastructure entity classes.
/// </summary>
public record UserRecord(
    int Id,
    string Email,
    string PasswordHash,
    UserRole Role,
    string FirstName,
    string LastName
);
