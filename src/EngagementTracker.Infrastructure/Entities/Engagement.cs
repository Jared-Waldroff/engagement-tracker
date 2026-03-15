using EngagementTracker.Core.Enums;

namespace EngagementTracker.Infrastructure.Entities;

/// <summary>
/// Represents a consulting engagement — a scoped project with a client,
/// assigned manager, budget, and timeline. Associates log time entries
/// against engagements to track hours and budget utilization.
/// </summary>
public class Engagement
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public EngagementStatus Status { get; set; } = EngagementStatus.Planning;
    public decimal BudgetHours { get; set; }
    public decimal HourlyRate { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>FK: the client this engagement is for.</summary>
    public int ClientId { get; set; }
    public Client Client { get; set; } = null!;

    /// <summary>FK: the manager responsible for this engagement.</summary>
    public int ManagerId { get; set; }
    public User Manager { get; set; } = null!;

    /// <summary>Navigation: all time entries logged against this engagement.</summary>
    public ICollection<TimeEntry> TimeEntries { get; set; } = new List<TimeEntry>();
}
