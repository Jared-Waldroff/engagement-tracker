namespace EngagementTracker.Infrastructure.Entities;

/// <summary>
/// Represents an external client organization. Each client can have
/// multiple engagements across different service lines.
/// </summary>
public class Client
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Industry { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Navigation: all engagements for this client.</summary>
    public ICollection<Engagement> Engagements { get; set; } = new List<Engagement>();
}
