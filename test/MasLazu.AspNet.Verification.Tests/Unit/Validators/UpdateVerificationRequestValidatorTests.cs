using FluentValidation.TestHelper;
using MasLazu.AspNet.Verification.Abstraction.Enums;
using MasLazu.AspNet.Verification.Abstraction.Models;
using MasLazu.AspNet.Verification.Validators;
using Xunit;

namespace MasLazu.AspNet.Verification.Tests.Unit.Validators;

public class UpdateVerificationRequestValidatorTests
{
    private readonly UpdateVerificationRequestValidator _validator;

    public UpdateVerificationRequestValidatorTests()
    {
        _validator = new UpdateVerificationRequestValidator();
    }

    [Fact]
    public void Validate_WithValidRequest_ShouldNotHaveValidationError()
    {
        // Arrange
        var request = new UpdateVerificationRequest(
            Id: Guid.NewGuid(),
            Channel: VerificationChannel.Email,
            Destination: "user@example.com",
            VerificationPurposeCode: "EMAIL_VERIFICATION",
            Status: VerificationStatus.Pending,
            ExpiresAt: DateTimeOffset.UtcNow.AddMinutes(30)
        );

        // Act
        TestValidationResult<UpdateVerificationRequest> result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithEmptyId_ShouldHaveValidationError()
    {
        // Arrange
        var request = new UpdateVerificationRequest(
            Id: Guid.Empty
        );

        // Act
        TestValidationResult<UpdateVerificationRequest> result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id)
              .WithErrorMessage("Verification ID is required.");
    }

    [Fact]
    public void Validate_WithInvalidChannel_ShouldHaveValidationError()
    {
        // Arrange
        var request = new UpdateVerificationRequest(
            Id: Guid.NewGuid(),
            Channel: (VerificationChannel)999
        );

        // Act
        TestValidationResult<UpdateVerificationRequest> result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Channel)
              .WithErrorMessage("Invalid verification channel.");
    }

    [Fact]
    public void Validate_WithNullChannel_ShouldNotHaveValidationError()
    {
        // Arrange
        var request = new UpdateVerificationRequest(
            Id: Guid.NewGuid(),
            Channel: null
        );

        // Act
        TestValidationResult<UpdateVerificationRequest> result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Channel);
    }

    [Fact]
    public void Validate_WithValidChannel_ShouldNotHaveValidationError()
    {
        // Arrange
        var request = new UpdateVerificationRequest(
            Id: Guid.NewGuid(),
            Channel: VerificationChannel.Email
        );

        // Act
        TestValidationResult<UpdateVerificationRequest> result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Channel);
    }

    [Fact]
    public void Validate_WithEmptyDestination_ShouldNotHaveValidationError()
    {
        // Arrange
        var request = new UpdateVerificationRequest(
            Id: Guid.NewGuid(),
            Destination: string.Empty
        );

        // Act
        TestValidationResult<UpdateVerificationRequest> result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Destination);
    }

    [Fact]
    public void Validate_WithNullDestination_ShouldNotHaveValidationError()
    {
        // Arrange
        var request = new UpdateVerificationRequest(
            Id: Guid.NewGuid(),
            Destination: null
        );

        // Act
        TestValidationResult<UpdateVerificationRequest> result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Destination);
    }

    [Theory]
    [InlineData("notanemail")]
    [InlineData("@example.com")]
    [InlineData("user@")]
    [InlineData("user")]
    public void Validate_WithInvalidEmailFormat_WhenChannelIsEmail_ShouldHaveValidationError(string invalidEmail)
    {
        // Arrange
        var request = new UpdateVerificationRequest(
            Id: Guid.NewGuid(),
            Channel: VerificationChannel.Email,
            Destination: invalidEmail
        );

        // Act
        TestValidationResult<UpdateVerificationRequest> result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Destination)
              .WithErrorMessage("Invalid email address format.");
    }

    [Theory]
    [InlineData("user@example.com")]
    [InlineData("user.name@example.com")]
    [InlineData("user+tag@example.co.uk")]
    public void Validate_WithValidEmailFormat_WhenChannelIsEmail_ShouldNotHaveValidationError(string validEmail)
    {
        // Arrange
        var request = new UpdateVerificationRequest(
            Id: Guid.NewGuid(),
            Channel: VerificationChannel.Email,
            Destination: validEmail
        );

        // Act
        TestValidationResult<UpdateVerificationRequest> result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Destination);
    }

    [Fact]
    public void Validate_WithInvalidEmailFormat_WhenChannelIsNotEmail_ShouldNotHaveValidationError()
    {
        // Arrange
        var request = new UpdateVerificationRequest(
            Id: Guid.NewGuid(),
            Channel: null,
            Destination: "notanemail"
        );

        // Act
        TestValidationResult<UpdateVerificationRequest> result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Destination);
    }

    [Fact]
    public void Validate_WithEmptyVerificationPurposeCode_ShouldNotHaveValidationError()
    {
        // Arrange
        var request = new UpdateVerificationRequest(
            Id: Guid.NewGuid(),
            VerificationPurposeCode: string.Empty
        );

        // Act
        TestValidationResult<UpdateVerificationRequest> result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.VerificationPurposeCode);
    }

    [Fact]
    public void Validate_WithNullVerificationPurposeCode_ShouldNotHaveValidationError()
    {
        // Arrange
        var request = new UpdateVerificationRequest(
            Id: Guid.NewGuid(),
            VerificationPurposeCode: null
        );

        // Act
        TestValidationResult<UpdateVerificationRequest> result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.VerificationPurposeCode);
    }

    [Fact]
    public void Validate_WithVerificationPurposeCodeExceeding50Characters_ShouldHaveValidationError()
    {
        // Arrange
        var request = new UpdateVerificationRequest(
            Id: Guid.NewGuid(),
            VerificationPurposeCode: new string('A', 51)
        );

        // Act
        TestValidationResult<UpdateVerificationRequest> result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.VerificationPurposeCode);
    }

    [Fact]
    public void Validate_WithInvalidStatus_ShouldHaveValidationError()
    {
        // Arrange
        var request = new UpdateVerificationRequest(
            Id: Guid.NewGuid(),
            Status: (VerificationStatus)999
        );

        // Act
        TestValidationResult<UpdateVerificationRequest> result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Status)
              .WithErrorMessage("Invalid verification status.");
    }

    [Fact]
    public void Validate_WithNullStatus_ShouldNotHaveValidationError()
    {
        // Arrange
        var request = new UpdateVerificationRequest(
            Id: Guid.NewGuid(),
            Status: null
        );

        // Act
        TestValidationResult<UpdateVerificationRequest> result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Status);
    }

    [Theory]
    [InlineData(VerificationStatus.Pending)]
    [InlineData(VerificationStatus.Verified)]
    [InlineData(VerificationStatus.Failed)]
    public void Validate_WithValidStatus_ShouldNotHaveValidationError(VerificationStatus validStatus)
    {
        // Arrange
        var request = new UpdateVerificationRequest(
            Id: Guid.NewGuid(),
            Status: validStatus
        );

        // Act
        TestValidationResult<UpdateVerificationRequest> result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Status);
    }

    [Fact]
    public void Validate_WithPastExpiresAt_ShouldHaveValidationError()
    {
        // Arrange
        var request = new UpdateVerificationRequest(
            Id: Guid.NewGuid(),
            ExpiresAt: DateTimeOffset.UtcNow.AddMinutes(-10)
        );

        // Act
        TestValidationResult<UpdateVerificationRequest> result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ExpiresAt)
              .WithErrorMessage("Expiration date must be in the future.");
    }

    [Fact]
    public void Validate_WithFutureExpiresAt_ShouldNotHaveValidationError()
    {
        // Arrange
        var request = new UpdateVerificationRequest(
            Id: Guid.NewGuid(),
            ExpiresAt: DateTimeOffset.UtcNow.AddMinutes(30)
        );

        // Act
        TestValidationResult<UpdateVerificationRequest> result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.ExpiresAt);
    }

    [Fact]
    public void Validate_WithNullExpiresAt_ShouldNotHaveValidationError()
    {
        // Arrange
        var request = new UpdateVerificationRequest(
            Id: Guid.NewGuid(),
            ExpiresAt: null
        );

        // Act
        TestValidationResult<UpdateVerificationRequest> result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.ExpiresAt);
    }
}
