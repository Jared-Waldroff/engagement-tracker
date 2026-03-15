namespace EngagementTracker.Infrastructure.Entities;

/// <summary>
/// Represents hours logged by an associate against a specific engagement.
/// Time entries are the core unit for budget tracking — the sum of hours
/// across all entries determines budget utilization.
/// </summary>
public class TimeEntry
{
    public int Id { get; set; }
    public decimal Hours { get; set; }
    public DateTime Date { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>FK: the user who logged this time entry.</summary>
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    /// <summary>FK: the engagement this time was logged against.</summary>
    public int EngagementId { get; set; }
    public Engagement Engagement { get; set; } = null!;
}
