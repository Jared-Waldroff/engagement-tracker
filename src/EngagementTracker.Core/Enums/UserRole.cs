namespace EngagementTracker.Core.Enums;

/// <summary>
/// Defines the access levels in the system.
/// Associates log time against engagements.
/// Managers oversee engagements and track budgets.
/// Partners view firm-wide billing summaries.
/// </summary>
public enum UserRole
{
    Associate,
    Manager,
    Partner
}
