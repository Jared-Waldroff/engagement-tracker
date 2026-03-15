using EngagementTracker.Core.Dtos;
using FluentValidation;

namespace EngagementTracker.Core.Validators;

/// <summary>
/// Validates engagement creation requests.
/// Ensures required fields are present and budget values are reasonable.
/// </summary>
public class CreateEngagementValidator : AbstractValidator<CreateEngagementDto>
{
    public CreateEngagementValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.ClientId)
            .GreaterThan(0).WithMessage("A valid client must be selected.");

        RuleFor(x => x.BudgetHours)
            .GreaterThan(0).WithMessage("Budget hours must be greater than zero.")
            .LessThanOrEqualTo(10000).WithMessage("Budget hours cannot exceed 10,000.");

        RuleFor(x => x.HourlyRate)
            .GreaterThan(0).WithMessage("Hourly rate must be greater than zero.")
            .LessThanOrEqualTo(1000).WithMessage("Hourly rate cannot exceed $1,000.");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Start date is required.");

        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate)
            .When(x => x.EndDate.HasValue)
            .WithMessage("End date must be after the start date.");
    }
}
