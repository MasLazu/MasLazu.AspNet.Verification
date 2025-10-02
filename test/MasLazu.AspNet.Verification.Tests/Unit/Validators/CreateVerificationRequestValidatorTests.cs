using FluentAssertions;
using FluentValidation.TestHelper;
using MasLazu.AspNet.Verification.Abstraction.Enums;
using MasLazu.AspNet.Verification.Abstraction.Models;
using MasLazu.AspNet.Verification.Tests.TestData;
using MasLazu.AspNet.Verification.Validators;

namespace MasLazu.AspNet.Verification.Tests.Unit.Validators;

/// <summary>
/// Unit tests for CreateVerificationRequestValidator
/// Tests all validation rules for creating verification requests
/// </summary>
public class CreateVerificationRequestValidatorTests
{
    private readonly CreateVerificationRequestValidator _validator;

    public CreateVerificationRequestValidatorTests()
    {
        _validator = new CreateVerificationRequestValidator();
    }

    [Fact]
    public void Validate_WithValidRequest_ShouldNotHaveValidationErrors()
    {
        // Arrange
        CreateVerificationRequest request = VerificationTestDataBuilder.CreateValidRequest();

        // Act
        TestValidationResult<CreateVerificationRequest> result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithEmptyUserId_ShouldHaveValidationError()
    {
        // Arrange
        var request = new CreateVerificationRequest(
            UserId: Guid.Empty,
            Channel: VerificationChannel.Email,
            Destination: "test@example.com",
            VerificationPurposeCode: "REGISTRATION",
            ExpiresAt: DateTimeOffset.UtcNow.AddMinutes(10)
        );

        // Act
        TestValidationResult<CreateVerificationRequest> result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UserId)
            .WithErrorMessage("User ID is required.");
    }

    [Fact]
    public void Validate_WithInvalidChannelEnum_ShouldHaveValidationError()
    {
        // Arrange
        var request = new CreateVerificationRequest(
            UserId: Guid.NewGuid(),
            Channel: (VerificationChannel)999,
            Destination: "test@example.com",
            VerificationPurposeCode: "REGISTRATION",
            ExpiresAt: null
        );

        // Act
        TestValidationResult<CreateVerificationRequest> result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Channel)
            .WithErrorMessage("Invalid verification channel.");
    }

    [Fact]
    public void Validate_WithEmptyDestination_ShouldHaveValidationError()
    {
        // Arrange
        var request = new CreateVerificationRequest(
            UserId: Guid.NewGuid(),
            Channel: VerificationChannel.Email,
            Destination: string.Empty,
            VerificationPurposeCode: "REGISTRATION",
            ExpiresAt: null
        );

        // Act
        TestValidationResult<CreateVerificationRequest> result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Destination)
            .WithErrorMessage("Destination is required.");
    }

    [Theory]
    [InlineData("notanemail")]
    [InlineData("@example.com")]
    [InlineData("user@")]
    [InlineData("user")]
    public void Validate_WithInvalidEmailFormat_WhenChannelIsEmail_ShouldHaveValidationError(string invalidEmail)
    {
        // Arrange
        var request = new CreateVerificationRequest(
            UserId: Guid.NewGuid(),
            Channel: VerificationChannel.Email,
            Destination: invalidEmail,
            VerificationPurposeCode: "REGISTRATION",
            ExpiresAt: null
        );

        // Act
        TestValidationResult<CreateVerificationRequest> result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Destination)
            .WithErrorMessage("Invalid email address format.");
    }

    [Theory]
    [InlineData("user@example.com")]
    [InlineData("user.name@example.com")]
    [InlineData("user+tag@example.co.uk")]
    [InlineData("user_name@mail.example.org")]
    public void Validate_WithValidEmailFormat_WhenChannelIsEmail_ShouldNotHaveValidationError(string validEmail)
    {
        // Arrange
        var request = new CreateVerificationRequest(
            UserId: Guid.NewGuid(),
            Channel: VerificationChannel.Email,
            Destination: validEmail,
            VerificationPurposeCode: "REGISTRATION",
            ExpiresAt: DateTimeOffset.UtcNow.AddMinutes(10)
        );

        // Act
        TestValidationResult<CreateVerificationRequest> result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Destination);
    }

    [Fact]
    public void Validate_WithEmptyPurposeCode_ShouldHaveValidationError()
    {
        // Arrange
        var request = new CreateVerificationRequest(
            UserId: Guid.NewGuid(),
            Channel: VerificationChannel.Email,
            Destination: "test@example.com",
            VerificationPurposeCode: string.Empty,
            ExpiresAt: null
        );

        // Act
        TestValidationResult<CreateVerificationRequest> result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.VerificationPurposeCode);
    }

    [Fact]
    public void Validate_WithPurposeCodeExceeding50Characters_ShouldHaveValidationError()
    {
        // Arrange
        string longCode = new string('A', 51);
        var request = new CreateVerificationRequest(
            UserId: Guid.NewGuid(),
            Channel: VerificationChannel.Email,
            Destination: "test@example.com",
            VerificationPurposeCode: longCode,
            ExpiresAt: null
        );

        // Act
        TestValidationResult<CreateVerificationRequest> result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.VerificationPurposeCode);
    }

    [Fact]
    public void Validate_WithPurposeCode50Characters_ShouldNotHaveValidationError()
    {
        // Arrange
        string code = new string('A', 50);
        var request = new CreateVerificationRequest(
            UserId: Guid.NewGuid(),
            Channel: VerificationChannel.Email,
            Destination: "test@example.com",
            VerificationPurposeCode: code,
            ExpiresAt: DateTimeOffset.UtcNow.AddMinutes(10)
        );

        // Act
        TestValidationResult<CreateVerificationRequest> result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.VerificationPurposeCode);
    }

    [Fact]
    public void Validate_WithExpiresAtInPast_ShouldHaveValidationError()
    {
        // Arrange
        var request = new CreateVerificationRequest(
            UserId: Guid.NewGuid(),
            Channel: VerificationChannel.Email,
            Destination: "test@example.com",
            VerificationPurposeCode: "REGISTRATION",
            ExpiresAt: DateTimeOffset.UtcNow.AddMinutes(-10)
        );

        // Act
        TestValidationResult<CreateVerificationRequest> result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ExpiresAt)
            .WithErrorMessage("Expiration date must be in the future.");
    }

    [Fact]
    public void Validate_WithExpiresAtInFuture_ShouldNotHaveValidationError()
    {
        // Arrange
        var request = new CreateVerificationRequest(
            UserId: Guid.NewGuid(),
            Channel: VerificationChannel.Email,
            Destination: "test@example.com",
            VerificationPurposeCode: "REGISTRATION",
            ExpiresAt: DateTimeOffset.UtcNow.AddMinutes(10)
        );

        // Act
        TestValidationResult<CreateVerificationRequest> result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.ExpiresAt);
    }

    [Fact]
    public void Validate_WithNullExpiresAt_ShouldNotHaveValidationError()
    {
        // Arrange
        var request = new CreateVerificationRequest(
            UserId: Guid.NewGuid(),
            Channel: VerificationChannel.Email,
            Destination: "test@example.com",
            VerificationPurposeCode: "REGISTRATION",
            ExpiresAt: null
        );

        // Act
        TestValidationResult<CreateVerificationRequest> result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.ExpiresAt);
    }
}
