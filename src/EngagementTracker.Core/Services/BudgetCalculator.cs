namespace EngagementTracker.Core.Services;

/// <summary>
/// Calculates budget metrics for engagements.
/// AtRisk threshold is 80% utilization. OverBudget is 100%+.
/// This is a pure static utility — no dependencies, fully testable.
/// </summary>
public static class BudgetCalculator
{
    /// <summary>
    /// Calculates budget utilization as a percentage.
    /// Returns 0 if budget hours is zero or negative to avoid divide-by-zero.
    /// </summary>
    /// <param name="hoursLogged">Total hours logged against the engagement.</param>
    /// <param name="budgetHours">Total budgeted hours for the engagement.</param>
    /// <returns>Utilization percentage rounded to one decimal place.</returns>
    public static decimal CalculateUtilization(decimal hoursLogged, decimal budgetHours)
    {
        if (budgetHours <= 0) return 0;
        return Math.Round(hoursLogged / budgetHours * 100, 1);
    }

    /// <summary>
    /// Determines the budget health status based on utilization percentage.
    /// OnTrack: below 80%. AtRisk: 80-99.9%. OverBudget: 100%+.
    /// </summary>
    /// <param name="utilizationPercent">The current utilization percentage.</param>
    /// <returns>A string status: "OnTrack", "AtRisk", or "OverBudget".</returns>
    public static string DetermineBudgetStatus(decimal utilizationPercent) => utilizationPercent switch
    {
        >= 100 => "OverBudget",
        >= 80 => "AtRisk",
        _ => "OnTrack"
    };
}
