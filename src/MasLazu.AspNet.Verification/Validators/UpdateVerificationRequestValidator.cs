using FluentValidation;
using MasLazu.AspNet.Verification.Abstraction.Models;
using MasLazu.AspNet.Verification.Abstraction.Enums;

namespace MasLazu.AspNet.Verification.Validators;

public class UpdateVerificationRequestValidator : AbstractValidator<UpdateVerificationRequest>
{
    public UpdateVerificationRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Verification ID is required.");

        RuleFor(x => x.Channel)
            .IsInEnum()
            .When(x => x.Channel.HasValue)
            .WithMessage("Invalid verification channel.");

        RuleFor(x => x.Destination)
            .NotEmpty()
            .When(x => !string.IsNullOrEmpty(x.Destination))
            .WithMessage("Destination cannot be empty when provided.");

        RuleFor(x => x.Destination)
            .EmailAddress()
            .When(x => x.Channel == VerificationChannel.Email && !string.IsNullOrEmpty(x.Destination))
            .WithMessage("Invalid email address format.");

        RuleFor(x => x.VerificationPurposeCode)
            .NotEmpty()
            .MaximumLength(50)
            .When(x => !string.IsNullOrEmpty(x.VerificationPurposeCode))
            .WithMessage("Verification purpose code cannot be empty and must not exceed 50 characters.");

        RuleFor(x => x.Status)
            .IsInEnum()
            .When(x => x.Status.HasValue)
            .WithMessage("Invalid verification status.");

        RuleFor(x => x.ExpiresAt)
            .GreaterThan(DateTimeOffset.UtcNow)
            .When(x => x.ExpiresAt.HasValue)
            .WithMessage("Expiration date must be in the future.");
    }
}
