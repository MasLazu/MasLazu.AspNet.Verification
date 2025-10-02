using FluentValidation.TestHelper;
using MasLazu.AspNet.Verification.Abstraction.Models;
using MasLazu.AspNet.Verification.Validators;
using Xunit;

namespace MasLazu.AspNet.Verification.Tests.Unit.Validators;

public class UpdateVerificationPurposeRequestValidatorTests
{
    private readonly UpdateVerificationPurposeRequestValidator _validator;

    public UpdateVerificationPurposeRequestValidatorTests()
    {
        _validator = new UpdateVerificationPurposeRequestValidator();
    }

    [Fact]
    public void Validate_WithValidRequest_ShouldNotHaveValidationError()
    {
        // Arrange
        var request = new UpdateVerificationPurposeRequest(
            Id: Guid.NewGuid(),
            Code: "EMAIL_VERIFICATION",
            Name: "Email Verification",
            Description: "Verification for email addresses"
        );

        // Act
        TestValidationResult<UpdateVerificationPurposeRequest> result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithMinimalValidRequest_ShouldNotHaveValidationError()
    {
        // Arrange
        var request = new UpdateVerificationPurposeRequest(
            Id: Guid.NewGuid()
        );

        // Act
        TestValidationResult<UpdateVerificationPurposeRequest> result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithEmptyId_ShouldHaveValidationError()
    {
        // Arrange
        var request = new UpdateVerificationPurposeRequest(
            Id: Guid.Empty
        );

        // Act
        TestValidationResult<UpdateVerificationPurposeRequest> result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id)
              .WithErrorMessage("Verification purpose ID is required.");
    }

    [Fact]
    public void Validate_WithEmptyCode_ShouldNotHaveValidationError()
    {
        // Arrange
        var request = new UpdateVerificationPurposeRequest(
            Id: Guid.NewGuid(),
            Code: string.Empty
        );

        // Act
        TestValidationResult<UpdateVerificationPurposeRequest> result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Code);
    }

    [Fact]
    public void Validate_WithNullCode_ShouldNotHaveValidationError()
    {
        // Arrange
        var request = new UpdateVerificationPurposeRequest(
            Id: Guid.NewGuid(),
            Code: null
        );

        // Act
        TestValidationResult<UpdateVerificationPurposeRequest> result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Code);
    }

    [Fact]
    public void Validate_WithCodeExceeding50Characters_ShouldHaveValidationError()
    {
        // Arrange
        var request = new UpdateVerificationPurposeRequest(
            Id: Guid.NewGuid(),
            Code: new string('A', 51)
        );

        // Act
        TestValidationResult<UpdateVerificationPurposeRequest> result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Code);
    }

    [Theory]
    [InlineData("email_verification")]  // lowercase
    [InlineData("Email-Verification")]  // dash instead of underscore
    [InlineData("EMAIL VERIFICATION")]  // space
    [InlineData("EMAIL@VERIFICATION")] // special character
    [InlineData("123_VERIFICATION")]   // starts with number
    public void Validate_WithInvalidCodeFormat_ShouldHaveValidationError(string invalidCode)
    {
        // Arrange
        var request = new UpdateVerificationPurposeRequest(
            Id: Guid.NewGuid(),
            Code: invalidCode
        );

        // Act
        TestValidationResult<UpdateVerificationPurposeRequest> result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Code);
    }

    [Theory]
    [InlineData("EMAIL_VERIFICATION")]
    [InlineData("PASSWORD_RESET")]
    [InlineData("TWO_FACTOR_AUTH")]
    [InlineData("ACCOUNT_RECOVERY")]
    [InlineData("A")]
    [InlineData("A_B_C")]
    public void Validate_WithValidCodeFormat_ShouldNotHaveValidationError(string validCode)
    {
        // Arrange
        var request = new UpdateVerificationPurposeRequest(
            Id: Guid.NewGuid(),
            Code: validCode
        );

        // Act
        TestValidationResult<UpdateVerificationPurposeRequest> result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Code);
    }

    [Fact]
    public void Validate_WithEmptyName_ShouldNotHaveValidationError()
    {
        // Arrange
        var request = new UpdateVerificationPurposeRequest(
            Id: Guid.NewGuid(),
            Name: string.Empty
        );

        // Act
        TestValidationResult<UpdateVerificationPurposeRequest> result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_WithNullName_ShouldNotHaveValidationError()
    {
        // Arrange
        var request = new UpdateVerificationPurposeRequest(
            Id: Guid.NewGuid(),
            Name: null
        );

        // Act
        TestValidationResult<UpdateVerificationPurposeRequest> result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_WithNameExceeding100Characters_ShouldHaveValidationError()
    {
        // Arrange
        var request = new UpdateVerificationPurposeRequest(
            Id: Guid.NewGuid(),
            Name: new string('A', 101)
        );

        // Act
        TestValidationResult<UpdateVerificationPurposeRequest> result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_WithName100Characters_ShouldNotHaveValidationError()
    {
        // Arrange
        var request = new UpdateVerificationPurposeRequest(
            Id: Guid.NewGuid(),
            Name: new string('A', 100)
        );

        // Act
        TestValidationResult<UpdateVerificationPurposeRequest> result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_WithNullDescription_ShouldNotHaveValidationError()
    {
        // Arrange
        var request = new UpdateVerificationPurposeRequest(
            Id: Guid.NewGuid(),
            Description: null
        );

        // Act
        TestValidationResult<UpdateVerificationPurposeRequest> result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Validate_WithEmptyDescription_ShouldNotHaveValidationError()
    {
        // Arrange
        var request = new UpdateVerificationPurposeRequest(
            Id: Guid.NewGuid(),
            Description: string.Empty
        );

        // Act
        TestValidationResult<UpdateVerificationPurposeRequest> result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Validate_WithDescriptionExceeding500Characters_ShouldHaveValidationError()
    {
        // Arrange
        var request = new UpdateVerificationPurposeRequest(
            Id: Guid.NewGuid(),
            Description: new string('A', 501)
        );

        // Act
        TestValidationResult<UpdateVerificationPurposeRequest> result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description)
              .WithErrorMessage("Description must not exceed 500 characters.");
    }

    [Fact]
    public void Validate_WithDescription500Characters_ShouldNotHaveValidationError()
    {
        // Arrange
        var request = new UpdateVerificationPurposeRequest(
            Id: Guid.NewGuid(),
            Description: new string('A', 500)
        );

        // Act
        TestValidationResult<UpdateVerificationPurposeRequest> result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }
}
