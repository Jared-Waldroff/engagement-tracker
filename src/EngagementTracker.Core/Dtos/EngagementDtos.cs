namespace EngagementTracker.Core.Dtos;

// ───────────────────────────────────────────
// Response DTOs — what the API returns
// ───────────────────────────────────────────

/// <summary>
/// Summary view of an engagement shown in list views and dashboards.
/// Includes computed budget metrics derived from time entries.
/// </summary>
public record EngagementSummaryDto(
    int Id,
    string ClientName,
    string Title,
    string Status,
    decimal BudgetHours,
    decimal HoursLogged,
    decimal BudgetUtilizationPercent,
    string BudgetStatus,
    string ManagerName,
    DateTime StartDate,
    DateTime? EndDate
);

/// <summary>
/// Detailed view of a single engagement including time entries,
/// per-user hour breakdown, and weekly trends.
/// </summary>
public record EngagementDetailDto(
    int Id,
    string ClientName,
    string ClientIndustry,
    string Title,
    string Description,
    string Status,
    decimal BudgetHours,
    decimal HourlyRate,
    decimal HoursLogged,
    decimal BudgetUtilizationPercent,
    string BudgetStatus,
    decimal TotalBudgetDollars,
    decimal SpentDollars,
    string ManagerName,
    DateTime StartDate,
    DateTime? EndDate,
    List<TimeEntryDto> RecentTimeEntries,
    List<UserHoursSummaryDto> HoursByUser
);

/// <summary>
/// Per-user summary of hours logged against a specific engagement.
/// Used in the engagement detail view's breakdown table.
/// </summary>
public record UserHoursSummaryDto(
    int UserId,
    string UserName,
    string Role,
    decimal TotalHours
);

// ───────────────────────────────────────────
// Request DTOs — what the client sends
// ───────────────────────────────────────────

/// <summary>
/// Filters for querying engagements. All fields are optional.
/// </summary>
public record EngagementFilterDto
{
    public string? Status { get; init; }
    public string? Search { get; init; }
    public DateTime? StartDateFrom { get; init; }
    public DateTime? StartDateTo { get; init; }
}

/// <summary>
/// Request body for creating a new engagement.
/// </summary>
public record CreateEngagementDto
{
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public int ClientId { get; init; }
    public decimal BudgetHours { get; init; }
    public decimal HourlyRate { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime? EndDate { get; init; }
}

/// <summary>
/// Request body for updating an existing engagement.
/// </summary>
public record UpdateEngagementDto
{
    public string? Title { get; init; }
    public string? Description { get; init; }
    public string? Status { get; init; }
    public decimal? BudgetHours { get; init; }
    public decimal? HourlyRate { get; init; }
    public DateTime? EndDate { get; init; }
}
