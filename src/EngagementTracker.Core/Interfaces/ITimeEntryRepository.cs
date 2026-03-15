using EngagementTracker.Core.Dtos;
using EngagementTracker.Core.Enums;

namespace EngagementTracker.Core.Interfaces;

/// <summary>
/// Repository abstraction for time entry data access.
/// </summary>
public interface ITimeEntryRepository
{
    /// <summary>
    /// Returns time entries visible to the user, optionally filtered by engagement.
    /// </summary>
    Task<List<TimeEntryDto>> GetTimeEntriesAsync(int userId, UserRole role, int? engagementId);

    /// <summary>
    /// Creates a new time entry and returns the DTO.
    /// </summary>
    Task<TimeEntryDto> CreateAsync(CreateTimeEntryDto dto, int userId);

    /// <summary>
    /// Checks whether an engagement exists.
    /// </summary>
    Task<bool> EngagementExistsAsync(int engagementId);
}
