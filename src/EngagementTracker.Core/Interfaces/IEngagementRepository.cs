using EngagementTracker.Core.Dtos;
using EngagementTracker.Core.Enums;

namespace EngagementTracker.Core.Interfaces;

/// <summary>
/// Repository abstraction for engagement data access.
/// Handles queries that require EF Core specifics (includes, filtering)
/// while returning DTOs to the service layer.
/// </summary>
public interface IEngagementRepository
{
    /// <summary>
    /// Returns engagement summaries visible to the specified user based on role.
    /// Applies optional filters for status, search text, and date range.
    /// </summary>
    Task<List<EngagementSummaryDto>> GetEngagementsAsync(int userId, UserRole role, EngagementFilterDto? filters);

    /// <summary>
    /// Returns a detailed engagement with time entries and user breakdowns.
    /// Returns null if not found.
    /// </summary>
    Task<EngagementDetailDto?> GetEngagementDetailAsync(int engagementId);

    /// <summary>
    /// Checks whether a user has access to a specific engagement based on their role.
    /// </summary>
    Task<bool> UserHasAccessAsync(int engagementId, int userId, UserRole role);

    /// <summary>
    /// Creates a new engagement and returns its summary.
    /// </summary>
    Task<EngagementSummaryDto> CreateAsync(CreateEngagementDto dto, int managerId);

    /// <summary>
    /// Updates an existing engagement and returns its updated summary.
    /// </summary>
    Task<EngagementSummaryDto> UpdateAsync(int engagementId, UpdateEngagementDto dto);

    /// <summary>
    /// Returns dashboard statistics for all engagements visible to the user.
    /// </summary>
    Task<DashboardDto> GetDashboardDataAsync(int userId, UserRole role);
}
