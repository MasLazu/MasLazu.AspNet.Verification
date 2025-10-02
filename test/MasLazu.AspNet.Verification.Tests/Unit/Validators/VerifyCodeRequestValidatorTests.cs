using FluentValidation.TestHelper;
using MasLazu.AspNet.Verification.Validators;
using Xunit;

namespace MasLazu.AspNet.Verification.Tests.Unit.Validators;

public class VerifyCodeRequestValidatorTests
{
    private readonly VerifyCodeRequestValidator _validator;

    public VerifyCodeRequestValidatorTests()
    {
        _validator = new VerifyCodeRequestValidator();
    }

    [Theory]
    [InlineData("123456")]
    [InlineData("000000")]
    [InlineData("999999")]
    public void Validate_WithValidCode_ShouldNotHaveValidationError(string validCode)
    {
        // Act
        TestValidationResult<string> result = _validator.TestValidate(validCode);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithEmptyCode_ShouldHaveValidationError()
    {
        // Arrange
        string code = string.Empty;

        // Act
        TestValidationResult<string> result = _validator.TestValidate(code);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x)
              .WithErrorMessage("Verification code is required.");
    }

    [Fact]
    public void Validate_WithNullCode_ShouldThrowException()
    {
        // Arrange
        string? code = null;

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _validator.TestValidate(code!));
    }

    [Theory]
    [InlineData("12345")]    // Too short
    [InlineData("1234567")]  // Too long
    [InlineData("1")]
    [InlineData("12")]
    public void Validate_WithInvalidLength_ShouldHaveValidationError(string invalidCode)
    {
        // Act
        TestValidationResult<string> result = _validator.TestValidate(invalidCode);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x)
              .WithErrorMessage("Verification code must be exactly 6 characters long.");
    }

    [Theory]
    [InlineData("12345a")]   // Contains letter
    [InlineData("abcdef")]   // All letters
    [InlineData("12 456")]   // Contains space
    [InlineData("123-56")]   // Contains dash
    [InlineData("123.56")]   // Contains dot
    public void Validate_WithNonNumericCharacters_ShouldHaveValidationError(string invalidCode)
    {
        // Act
        TestValidationResult<string> result = _validator.TestValidate(invalidCode);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x)
              .WithErrorMessage("Verification code must contain only numbers.");
    }
}
