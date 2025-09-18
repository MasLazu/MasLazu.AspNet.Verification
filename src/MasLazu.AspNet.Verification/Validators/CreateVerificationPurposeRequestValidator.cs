using FluentValidation;
using MasLazu.AspNet.Verification.Abstraction.Models;

namespace MasLazu.AspNet.Verification.Validators;

public class CreateVerificationPurposeRequestValidator : AbstractValidator<CreateVerificationPurposeRequest>
{
    public CreateVerificationPurposeRequestValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .MaximumLength(50)
            .Matches(@"^[A-Z_]+$")
            .WithMessage("Code is required, must not exceed 50 characters, and should contain only uppercase letters and underscores.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("Name is required and must not exceed 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.Description))
            .WithMessage("Description must not exceed 500 characters.");
    }
}
