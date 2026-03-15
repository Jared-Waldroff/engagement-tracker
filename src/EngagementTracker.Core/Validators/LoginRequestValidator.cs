using EngagementTracker.Core.Dtos;
using FluentValidation;

namespace EngagementTracker.Core.Validators;

/// <summary>
/// Validates login requests. Ensures email format and password presence.
/// </summary>
public class LoginRequestValidator : AbstractValidator<LoginRequestDto>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
}
