using FluentValidation;
using MasLazu.AspNet.Verification.Abstraction.Models;

namespace MasLazu.AspNet.Verification.Validators;

public class UpdateVerificationPurposeRequestValidator : AbstractValidator<UpdateVerificationPurposeRequest>
{
    public UpdateVerificationPurposeRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Verification purpose ID is required.");

        RuleFor(x => x.Code)
            .NotEmpty()
            .MaximumLength(50)
            .Matches(@"^[A-Z_]+$")
            .When(x => !string.IsNullOrEmpty(x.Code))
            .WithMessage("Code cannot be empty when provided, must not exceed 50 characters, and should contain only uppercase letters and underscores.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100)
            .When(x => !string.IsNullOrEmpty(x.Name))
            .WithMessage("Name cannot be empty when provided and must not exceed 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.Description))
            .WithMessage("Description must not exceed 500 characters.");
    }
}
