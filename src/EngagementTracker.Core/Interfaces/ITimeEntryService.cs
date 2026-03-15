using EngagementTracker.Core.Dtos;
using EngagementTracker.Core.Enums;

namespace EngagementTracker.Core.Interfaces;

/// <summary>
/// Service for managing time entries. Associates log time against engagements.
/// Managers and Partners can view time entries for engagements they have access to.
/// </summary>
public interface ITimeEntryService
{
    /// <summary>
    /// Returns time entries visible to the user, filtered by role.
    /// Associates see their own entries. Managers see entries for their engagements.
    /// Partners see all entries.
    /// </summary>
    Task<List<TimeEntryDto>> GetTimeEntriesAsync(int userId, UserRole role, int? engagementId = null);

    /// <summary>
    /// Logs a new time entry for the authenticated user.
    /// Validates that the engagement exists and the user has access.
    /// </summary>
    /// <param name="dto">Time entry data.</param>
    /// <param name="userId">The authenticated user logging time.</param>
    /// <returns>The created time entry.</returns>
    Task<TimeEntryDto> CreateTimeEntryAsync(CreateTimeEntryDto dto, int userId);
}
