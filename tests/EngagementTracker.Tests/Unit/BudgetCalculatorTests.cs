using EngagementTracker.Core.Services;

namespace EngagementTracker.Tests.Unit;

/// <summary>
/// Tests the BudgetCalculator static utility that determines budget utilization
/// and health status. These are pure functions with no dependencies.
/// </summary>
public class BudgetCalculatorTests
{
    [Theory]
    [InlineData(40, 100, 40.0)]    // 40% utilization
    [InlineData(80, 100, 80.0)]    // AtRisk threshold
    [InlineData(100, 100, 100.0)]  // Exactly at budget
    [InlineData(120, 100, 120.0)]  // Over budget
    [InlineData(0, 100, 0.0)]      // No hours logged
    [InlineData(50, 0, 0.0)]       // Zero budget — avoid divide by zero
    [InlineData(10, -5, 0.0)]      // Negative budget — treated as zero
    [InlineData(0.5, 1, 50.0)]     // Fractional hours
    public void CalculateUtilization_ReturnsCorrectPercentage(
        decimal hoursLogged, decimal budgetHours, decimal expected)
    {
        decimal result = BudgetCalculator.CalculateUtilization(hoursLogged, budgetHours);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(0.0, "OnTrack")]
    [InlineData(50.0, "OnTrack")]
    [InlineData(79.9, "OnTrack")]
    [InlineData(80.0, "AtRisk")]
    [InlineData(90.0, "AtRisk")]
    [InlineData(99.9, "AtRisk")]
    [InlineData(100.0, "OverBudget")]
    [InlineData(150.0, "OverBudget")]
    public void DetermineBudgetStatus_ReturnsCorrectStatus(
        decimal utilization, string expected)
    {
        string result = BudgetCalculator.DetermineBudgetStatus(utilization);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void CalculateUtilization_RoundsToOneDecimalPlace()
    {
        // 33 / 100 = 33.333...% → should round to 33.3
        decimal result = BudgetCalculator.CalculateUtilization(33.333m, 100);
        Assert.Equal(33.3m, result);
    }
}
