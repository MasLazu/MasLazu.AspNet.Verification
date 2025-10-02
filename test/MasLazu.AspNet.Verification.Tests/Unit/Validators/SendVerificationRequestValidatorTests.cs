using FluentValidation.TestHelper;
using MasLazu.AspNet.Verification.Abstraction.Models;
using MasLazu.AspNet.Verification.Validators;
using Xunit;

namespace MasLazu.AspNet.Verification.Tests.Unit.Validators;

public class SendVerificationRequestValidatorTests
{
    private readonly SendVerificationRequestValidator _validator;

    public SendVerificationRequestValidatorTests()
    {
        _validator = new SendVerificationRequestValidator();
    }

    [Fact]
    public void Validate_WithValidRequest_ShouldNotHaveValidationError()
    {
        // Arrange
        var request = new SendVerificationRequest(
            UserId: Guid.NewGuid(),
            Destination: "user@example.com",
            PurposeCode: "EMAIL_VERIFICATION",
            ExpiresAt: DateTimeOffset.UtcNow.AddMinutes(30)
        );

        // Act
        TestValidationResult<SendVerificationRequest> result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithEmptyUserId_ShouldHaveValidationError()
    {
        // Arrange
        var request = new SendVerificationRequest(
            UserId: Guid.Empty,
            Destination: "user@example.com",
            PurposeCode: "EMAIL_VERIFICATION"
        );

        // Act
        TestValidationResult<SendVerificationRequest> result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UserId)
              .WithErrorMessage("User ID is required.");
    }

    [Fact]
    public void Validate_WithEmptyDestination_ShouldHaveValidationError()
    {
        // Arrange
        var request = new SendVerificationRequest(
            UserId: Guid.NewGuid(),
            Destination: string.Empty,
            PurposeCode: "EMAIL_VERIFICATION"
        );

        // Act
        TestValidationResult<SendVerificationRequest> result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Destination)
              .WithErrorMessage("Destination is required.");
    }

    [Theory]
    [InlineData("notanemail")]
    [InlineData("@example.com")]
    [InlineData("user@")]
    [InlineData("user")]
    public void Validate_WithInvalidEmailFormat_ShouldHaveValidationError(string invalidEmail)
    {
        // Arrange
        var request = new SendVerificationRequest(
            UserId: Guid.NewGuid(),
            Destination: invalidEmail,
            PurposeCode: "EMAIL_VERIFICATION"
        );

        // Act
        TestValidationResult<SendVerificationRequest> result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Destination)
              .WithErrorMessage("Invalid email address format.");
    }

    [Theory]
    [InlineData("user@example.com")]
    [InlineData("user.name@example.com")]
    [InlineData("user+tag@example.co.uk")]
    public void Validate_WithValidEmailFormat_ShouldNotHaveValidationError(string validEmail)
    {
        // Arrange
        var request = new SendVerificationRequest(
            UserId: Guid.NewGuid(),
            Destination: validEmail,
            PurposeCode: "EMAIL_VERIFICATION"
        );

        // Act
        TestValidationResult<SendVerificationRequest> result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Destination);
    }

    [Fact]
    public void Validate_WithEmptyPurposeCode_ShouldHaveValidationError()
    {
        // Arrange
        var request = new SendVerificationRequest(
            UserId: Guid.NewGuid(),
            Destination: "test@example.com",
            PurposeCode: string.Empty,
            ExpiresAt: DateTimeOffset.UtcNow.AddMinutes(30)
        );

        // Act
        TestValidationResult<SendVerificationRequest> result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PurposeCode);
    }

    [Fact]
    public void Validate_WithPurposeCodeExceeding50Characters_ShouldHaveValidationError()
    {
        // Arrange
        var request = new SendVerificationRequest(
            UserId: Guid.NewGuid(),
            Destination: "user@example.com",
            PurposeCode: new string('A', 51)
        );

        // Act
        TestValidationResult<SendVerificationRequest> result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PurposeCode)
              .WithErrorMessage("Purpose code is required and must not exceed 50 characters.");
    }

    [Fact]
    public void Validate_WithPurposeCode50Characters_ShouldNotHaveValidationError()
    {
        // Arrange
        var request = new SendVerificationRequest(
            UserId: Guid.NewGuid(),
            Destination: "user@example.com",
            PurposeCode: new string('A', 50)
        );

        // Act
        TestValidationResult<SendVerificationRequest> result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.PurposeCode);
    }

    [Fact]
    public void Validate_WithPastExpiresAt_ShouldHaveValidationError()
    {
        // Arrange
        var request = new SendVerificationRequest(
            UserId: Guid.NewGuid(),
            Destination: "user@example.com",
            PurposeCode: "EMAIL_VERIFICATION",
            ExpiresAt: DateTimeOffset.UtcNow.AddMinutes(-10)
        );

        // Act
        TestValidationResult<SendVerificationRequest> result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ExpiresAt)
              .WithErrorMessage("Expiration date must be in the future.");
    }

    [Fact]
    public void Validate_WithFutureExpiresAt_ShouldNotHaveValidationError()
    {
        // Arrange
        var request = new SendVerificationRequest(
            UserId: Guid.NewGuid(),
            Destination: "user@example.com",
            PurposeCode: "EMAIL_VERIFICATION",
            ExpiresAt: DateTimeOffset.UtcNow.AddMinutes(30)
        );

        // Act
        TestValidationResult<SendVerificationRequest> result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.ExpiresAt);
    }

    [Fact]
    public void Validate_WithNullExpiresAt_ShouldNotHaveValidationError()
    {
        // Arrange
        var request = new SendVerificationRequest(
            UserId: Guid.NewGuid(),
            Destination: "user@example.com",
            PurposeCode: "EMAIL_VERIFICATION",
            ExpiresAt: null
        );

        // Act
        TestValidationResult<SendVerificationRequest> result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.ExpiresAt);
    }
}
