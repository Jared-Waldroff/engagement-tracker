namespace EngagementTracker.Core.Enums;

/// <summary>
/// Tracks the lifecycle of a client engagement from planning through completion.
/// </summary>
public enum EngagementStatus
{
    Planning,
    Active,
    OnHold,
    Completed,
    Cancelled
}
