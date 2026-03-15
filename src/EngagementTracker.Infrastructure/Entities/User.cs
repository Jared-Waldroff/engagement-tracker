using EngagementTracker.Core.Enums;

namespace EngagementTracker.Infrastructure.Entities;

/// <summary>
/// Represents a user in the system. Users are assigned a role that determines
/// their access level: Associates log time, Managers oversee engagements,
/// Partners view firm-wide billing summaries.
/// </summary>
public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Navigation: engagements this user manages (only relevant for Managers).</summary>
    public ICollection<Engagement> ManagedEngagements { get; set; } = new List<Engagement>();

    /// <summary>Navigation: time entries logged by this user.</summary>
    public ICollection<TimeEntry> TimeEntries { get; set; } = new List<TimeEntry>();
}
