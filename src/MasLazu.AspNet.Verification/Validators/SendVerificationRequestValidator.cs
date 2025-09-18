using FluentValidation;
using MasLazu.AspNet.Verification.Abstraction.Models;

namespace MasLazu.AspNet.Verification.Validators;

public class SendVerificationRequestValidator : AbstractValidator<SendVerificationRequest>
{
    public SendVerificationRequestValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required.");

        RuleFor(x => x.Destination)
            .NotEmpty()
            .WithMessage("Destination is required.");

        RuleFor(x => x.Destination)
            .EmailAddress()
            .WithMessage("Invalid email address format.");

        RuleFor(x => x.PurposeCode)
            .NotEmpty()
            .MaximumLength(50)
            .WithMessage("Purpose code is required and must not exceed 50 characters.");

        RuleFor(x => x.ExpiresAt)
            .GreaterThan(DateTimeOffset.UtcNow)
            .When(x => x.ExpiresAt.HasValue)
            .WithMessage("Expiration date must be in the future.");
    }
}
