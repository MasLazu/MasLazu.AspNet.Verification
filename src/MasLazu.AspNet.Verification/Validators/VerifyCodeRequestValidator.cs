using FluentValidation;

namespace MasLazu.AspNet.Verification.Validators;

public class VerifyCodeRequestValidator : AbstractValidator<string>
{
    public VerifyCodeRequestValidator()
    {
        RuleFor(code => code)
            .NotEmpty()
            .WithMessage("Verification code is required.")
            .Length(6)
            .WithMessage("Verification code must be exactly 6 characters long.")
            .Matches(@"^[0-9]+$")
            .WithMessage("Verification code must contain only numbers.");
    }
}
