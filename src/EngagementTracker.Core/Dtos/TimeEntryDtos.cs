namespace EngagementTracker.Core.Dtos;

// ───────────────────────────────────────────
// Response DTOs
// ───────────────────────────────────────────

/// <summary>
/// Represents a single time entry in API responses.
/// </summary>
public record TimeEntryDto(
    int Id,
    int EngagementId,
    string EngagementTitle,
    string UserName,
    decimal Hours,
    DateTime Date,
    string Description
);

// ───────────────────────────────────────────
// Request DTOs
// ───────────────────────────────────────────

/// <summary>
/// Request body for logging a new time entry against an engagement.
/// </summary>
public record CreateTimeEntryDto
{
    public int EngagementId { get; init; }
    public decimal Hours { get; init; }
    public DateTime Date { get; init; }
    public string Description { get; init; } = string.Empty;
}
