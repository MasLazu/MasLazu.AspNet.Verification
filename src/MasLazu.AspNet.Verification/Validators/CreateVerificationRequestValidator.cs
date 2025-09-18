using FluentValidation;
using MasLazu.AspNet.Verification.Abstraction.Models;
using MasLazu.AspNet.Verification.Abstraction.Enums;

namespace MasLazu.AspNet.Verification.Validators;

public class CreateVerificationRequestValidator : AbstractValidator<CreateVerificationRequest>
{
    public CreateVerificationRequestValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required.");

        RuleFor(x => x.Channel)
            .IsInEnum()
            .WithMessage("Invalid verification channel.");

        RuleFor(x => x.Destination)
            .NotEmpty()
            .WithMessage("Destination is required.");

        RuleFor(x => x.Destination)
            .EmailAddress()
            .When(x => x.Channel == VerificationChannel.Email)
            .WithMessage("Invalid email address format.");

        RuleFor(x => x.VerificationPurposeCode)
            .NotEmpty()
            .MaximumLength(50)
            .WithMessage("Verification purpose code is required and must not exceed 50 characters.");

        RuleFor(x => x.ExpiresAt)
            .GreaterThan(DateTimeOffset.UtcNow)
            .When(x => x.ExpiresAt.HasValue)
            .WithMessage("Expiration date must be in the future.");
    }
}
