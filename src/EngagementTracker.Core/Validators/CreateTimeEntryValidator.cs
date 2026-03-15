using EngagementTracker.Core.Dtos;
using FluentValidation;

namespace EngagementTracker.Core.Validators;

/// <summary>
/// Validates time entry creation requests.
/// Ensures hours are within a reasonable daily range and the date is not in the future.
/// </summary>
public class CreateTimeEntryValidator : AbstractValidator<CreateTimeEntryDto>
{
    public CreateTimeEntryValidator()
    {
        RuleFor(x => x.EngagementId)
            .GreaterThan(0).WithMessage("A valid engagement must be selected.");

        RuleFor(x => x.Hours)
            .GreaterThan(0).WithMessage("Hours must be greater than zero.")
            .LessThanOrEqualTo(24).WithMessage("Hours cannot exceed 24 per entry.");

        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("Date is required.")
            .LessThanOrEqualTo(DateTime.UtcNow.Date.AddDays(1))
            .WithMessage("Date cannot be in the future.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");
    }
}
