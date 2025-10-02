using FluentValidation.TestHelper;
using MasLazu.AspNet.Verification.Abstraction.Models;
using MasLazu.AspNet.Verification.Validators;
using Xunit;

namespace MasLazu.AspNet.Verification.Tests.Unit.Validators;

public class CreateVerificationPurposeRequestValidatorTests
{
    private readonly CreateVerificationPurposeRequestValidator _validator;

    public CreateVerificationPurposeRequestValidatorTests()
    {
        _validator = new CreateVerificationPurposeRequestValidator();
    }

    [Fact]
    public void Validate_WithValidRequest_ShouldNotHaveValidationError()
    {
        // Arrange
        var request = new CreateVerificationPurposeRequest(
            Code: "EMAIL_VERIFICATION",
            Name: "Email Verification",
            Description: "Verification for email addresses"
        );

        // Act
        TestValidationResult<CreateVerificationPurposeRequest> result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithMinimalValidRequest_ShouldNotHaveValidationError()
    {
        // Arrange
        var request = new CreateVerificationPurposeRequest(
            Code: "TEST",
            Name: "Test",
            Description: null
        );

        // Act
        TestValidationResult<CreateVerificationPurposeRequest> result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithEmptyCode_ShouldHaveValidationError()
    {
        // Arrange
        var request = new CreateVerificationPurposeRequest(
            Code: string.Empty,
            Name: "Email Verification",
            Description: null
        );

        // Act
        TestValidationResult<CreateVerificationPurposeRequest> result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Code);
    }

    [Fact]
    public void Validate_WithCodeExceeding50Characters_ShouldHaveValidationError()
    {
        // Arrange
        var request = new CreateVerificationPurposeRequest(
            Code: new string('A', 51),
            Name: "Email Verification",
            Description: null
        );

        // Act
        TestValidationResult<CreateVerificationPurposeRequest> result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Code);
    }

    [Fact]
    public void Validate_WithCode50Characters_ShouldNotHaveValidationError()
    {
        // Arrange
        var request = new CreateVerificationPurposeRequest(
            Code: new string('A', 50),
            Name: "Email Verification",
            Description: null
        );

        // Act
        TestValidationResult<CreateVerificationPurposeRequest> result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Code);
    }

    [Fact]
    public void Validate_WithEmptyName_ShouldHaveValidationError()
    {
        // Arrange
        var request = new CreateVerificationPurposeRequest(
            Code: "EMAIL_VERIFICATION",
            Name: string.Empty,
            Description: null
        );

        // Act
        TestValidationResult<CreateVerificationPurposeRequest> result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_WithNameExceeding100Characters_ShouldHaveValidationError()
    {
        // Arrange
        var request = new CreateVerificationPurposeRequest(
            Code: "EMAIL_VERIFICATION",
            Name: new string('A', 101),
            Description: null
        );

        // Act
        TestValidationResult<CreateVerificationPurposeRequest> result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
              .WithErrorMessage("Name is required and must not exceed 100 characters.");
    }

    [Fact]
    public void Validate_WithName100Characters_ShouldNotHaveValidationError()
    {
        // Arrange
        var request = new CreateVerificationPurposeRequest(
            Code: "EMAIL_VERIFICATION",
            Name: new string('A', 100),
            Description: null
        );

        // Act
        TestValidationResult<CreateVerificationPurposeRequest> result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_WithNullDescription_ShouldNotHaveValidationError()
    {
        // Arrange
        var request = new CreateVerificationPurposeRequest(
            Code: "EMAIL_VERIFICATION",
            Name: "Email Verification",
            Description: null
        );

        // Act
        TestValidationResult<CreateVerificationPurposeRequest> result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Validate_WithEmptyDescription_ShouldNotHaveValidationError()
    {
        // Arrange
        var request = new CreateVerificationPurposeRequest(
            Code: "EMAIL_VERIFICATION",
            Name: "Email Verification",
            Description: string.Empty
        );

        // Act
        TestValidationResult<CreateVerificationPurposeRequest> result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Validate_WithDescriptionExceeding500Characters_ShouldHaveValidationError()
    {
        // Arrange
        var request = new CreateVerificationPurposeRequest(
            Code: "EMAIL_VERIFICATION",
            Name: "Email Verification",
            Description: new string('A', 501)
        );

        // Act
        TestValidationResult<CreateVerificationPurposeRequest> result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description)
              .WithErrorMessage("Description must not exceed 500 characters.");
    }

    [Fact]
    public void Validate_WithDescription500Characters_ShouldNotHaveValidationError()
    {
        // Arrange
        var request = new CreateVerificationPurposeRequest(
            Code: "EMAIL_VERIFICATION",
            Name: "Email Verification",
            Description: new string('A', 500)
        );

        // Act
        TestValidationResult<CreateVerificationPurposeRequest> result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }
}
