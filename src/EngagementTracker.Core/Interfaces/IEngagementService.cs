using EngagementTracker.Core.Dtos;
using EngagementTracker.Core.Enums;

namespace EngagementTracker.Core.Interfaces;

/// <summary>
/// Service for managing client engagements. Enforces role-based access:
/// Partners see all engagements, Managers see their managed engagements,
/// Associates see engagements they've logged time against.
/// </summary>
public interface IEngagementService
{
    /// <summary>
    /// Returns engagement summaries filtered by the user's role and optional filters.
    /// Includes computed fields: total hours logged, budget utilization percentage,
    /// and budget status (OnTrack, AtRisk, OverBudget).
    /// </summary>
    /// <param name="userId">The authenticated user's ID from the JWT claim.</param>
    /// <param name="role">The user's role (Associate, Manager, Partner).</param>
    /// <param name="filters">Optional filters for status, search, and date range.</param>
    /// <returns>A list of engagement summaries with budget utilization percentages.</returns>
    Task<List<EngagementSummaryDto>> GetEngagementsAsync(int userId, UserRole role, EngagementFilterDto? filters = null);

    /// <summary>
    /// Returns a detailed view of a single engagement including all time entries,
    /// budget breakdown by user, and weekly hour trends.
    /// </summary>
    /// <param name="engagementId">The engagement ID to retrieve.</param>
    /// <param name="userId">The authenticated user's ID.</param>
    /// <param name="role">The user's role.</param>
    /// <returns>Full engagement detail with time entries and user breakdowns.</returns>
    /// <exception cref="Exceptions.NotFoundException">Thrown when the engagement doesn't exist.</exception>
    /// <exception cref="Exceptions.ForbiddenException">Thrown when the user doesn't have access.</exception>
    Task<EngagementDetailDto> GetEngagementDetailAsync(int engagementId, int userId, UserRole role);

    /// <summary>
    /// Creates a new engagement assigned to the specified manager.
    /// </summary>
    /// <param name="dto">The engagement creation data.</param>
    /// <param name="managerId">The ID of the manager creating the engagement.</param>
    /// <returns>The created engagement summary.</returns>
    Task<EngagementSummaryDto> CreateEngagementAsync(CreateEngagementDto dto, int managerId);

    /// <summary>
    /// Updates an existing engagement. Only managers of the engagement and partners can update.
    /// </summary>
    /// <param name="engagementId">The engagement to update.</param>
    /// <param name="dto">The fields to update.</param>
    /// <param name="userId">The authenticated user's ID.</param>
    /// <param name="role">The user's role.</param>
    /// <returns>The updated engagement summary.</returns>
    Task<EngagementSummaryDto> UpdateEngagementAsync(int engagementId, UpdateEngagementDto dto, int userId, UserRole role);

    /// <summary>
    /// Returns dashboard statistics tailored to the user's role.
    /// </summary>
    Task<DashboardDto> GetDashboardAsync(int userId, UserRole role);
}
