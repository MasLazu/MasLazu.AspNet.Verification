using FluentAssertions;
using FluentValidation;
using MasLazu.AspNet.Framework.Application.Interfaces;
using MasLazu.AspNet.Verification.Abstraction.Models;
using MasLazu.AspNet.Verification.Domain.Entities;
using MasLazu.AspNet.Verification.Services;
using Moq;
using System.Linq.Expressions;
using Xunit;
using TestMockFactory = MasLazu.AspNet.Verification.Tests.Fixtures.MockFactory;
using FastEndpoints;

namespace MasLazu.AspNet.Verification.Tests.Unit.Services;

public class VerificationPurposeServiceTests
{
    private readonly Mock<IRepository<VerificationPurpose>> _repositoryMock;
    private readonly Mock<IReadRepository<VerificationPurpose>> _readRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IValidator<CreateVerificationPurposeRequest>> _createValidatorMock;
    private readonly Mock<IValidator<UpdateVerificationPurposeRequest>> _updateValidatorMock;
    private readonly VerificationPurposeService _service;

    public VerificationPurposeServiceTests()
    {
        _repositoryMock = TestMockFactory.CreateRepositoryMock<VerificationPurpose>();
        _readRepositoryMock = TestMockFactory.CreateReadRepositoryMock<VerificationPurpose>();
        _unitOfWorkMock = TestMockFactory.CreateUnitOfWorkMock();
        _createValidatorMock = TestMockFactory.CreateValidatorMock<CreateVerificationPurposeRequest>();
        _updateValidatorMock = TestMockFactory.CreateValidatorMock<UpdateVerificationPurposeRequest>();

        _service = new VerificationPurposeService(
            _repositoryMock.Object,
            _readRepositoryMock.Object,
            _unitOfWorkMock.Object,
            TestMockFactory.CreatePropertyMapMock<VerificationPurpose>().Object,
            TestMockFactory.CreatePaginationValidatorMock<VerificationPurpose>().Object,
            TestMockFactory.CreateCursorPaginationValidatorMock<VerificationPurpose>().Object,
            _createValidatorMock.Object,
            _updateValidatorMock.Object
        );
    }

    #region CreateIfNotExistsAsync Tests

    [Fact]
    public async Task CreateIfNotExistsAsync_WhenEntityDoesNotExist_ShouldCreateNewEntity()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new CreateVerificationPurposeRequest(
            Code: "EMAIL_VERIFICATION",
            Name: "Email Verification",
            Description: "Verification for email addresses"
        );

        _readRepositoryMock
            .Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<VerificationPurpose, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((VerificationPurpose?)null);

        VerificationPurpose? capturedEntity = null;
        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<VerificationPurpose>(), It.IsAny<CancellationToken>()))
            .Callback<VerificationPurpose, CancellationToken>((entity, ct) => capturedEntity = entity)
            .ReturnsAsync((VerificationPurpose entity, CancellationToken ct) => entity);

        // Act
        VerificationPurposeDto result = await _service.CreateIfNotExistsAsync(id, request);

        // Assert
        result.Should().NotBeNull();
        result.Code.Should().Be("EMAIL_VERIFICATION");
        result.Name.Should().Be("Email Verification");
        result.Description.Should().Be("Verification for email addresses");

        capturedEntity.Should().NotBeNull();
        capturedEntity!.Id.Should().Be(id);
        capturedEntity.Code.Should().Be("EMAIL_VERIFICATION");

        _repositoryMock.Verify(r => r.AddAsync(
            It.Is<VerificationPurpose>(vp => vp.Id == id && vp.Code == "EMAIL_VERIFICATION"),
            It.IsAny<CancellationToken>()), Times.Once);

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateIfNotExistsAsync_WhenEntityExists_ShouldReturnExistingEntity()
    {
        // Arrange
        var id = Guid.NewGuid();
        var existingEntity = new VerificationPurpose
        {
            Id = id,
            Code = "EMAIL_VERIFICATION",
            Name = "Email Verification",
            Description = "Existing verification purpose",
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow.AddDays(-10)
        };

        var request = new CreateVerificationPurposeRequest(
            Code: "EMAIL_VERIFICATION",
            Name: "Email Verification",
            Description: "This should not be used"
        );

        _readRepositoryMock
            .Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<VerificationPurpose, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingEntity);

        // Act
        VerificationPurposeDto result = await _service.CreateIfNotExistsAsync(id, request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(id);
        result.Code.Should().Be("EMAIL_VERIFICATION");
        result.Name.Should().Be("Email Verification");
        result.Description.Should().Be("Existing verification purpose");

        _repositoryMock.Verify(r => r.AddAsync(
            It.IsAny<VerificationPurpose>(),
            It.IsAny<CancellationToken>()), Times.Never);

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateIfNotExistsAsync_ShouldValidateRequest()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new CreateVerificationPurposeRequest(
            Code: "EMAIL_VERIFICATION",
            Name: "Email Verification",
            Description: "Test description"
        );

        _readRepositoryMock
            .Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<VerificationPurpose, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((VerificationPurpose?)null);

        // Act
        await _service.CreateIfNotExistsAsync(id, request);

        // Assert
        _createValidatorMock.Verify(v => v.ValidateAsync(
            It.Is<CreateVerificationPurposeRequest>(r => r.Code == "EMAIL_VERIFICATION"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateIfNotExistsAsync_WithInvalidRequest_ShouldThrowValidationException()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new CreateVerificationPurposeRequest(
            Code: string.Empty, // Invalid
            Name: "Email Verification",
            Description: "Test description"
        );

        var validationResult = new FluentValidation.Results.ValidationResult(
            new[] { new FluentValidation.Results.ValidationFailure("Code", "Code is required") }
        );

        _createValidatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<CreateVerificationPurposeRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        // Act & Assert
        await Assert.ThrowsAsync<ValidationFailureException>(() =>
            _service.CreateIfNotExistsAsync(id, request));

        _repositoryMock.Verify(r => r.AddAsync(
            It.IsAny<VerificationPurpose>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateIfNotExistsAsync_ShouldSetIdCorrectly()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new CreateVerificationPurposeRequest(
            Code: "PASSWORD_RESET",
            Name: "Password Reset",
            Description: "Reset user password"
        );

        _readRepositoryMock
            .Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<VerificationPurpose, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((VerificationPurpose?)null);

        VerificationPurpose? capturedEntity = null;
        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<VerificationPurpose>(), It.IsAny<CancellationToken>()))
            .Callback<VerificationPurpose, CancellationToken>((entity, ct) => capturedEntity = entity)
            .ReturnsAsync((VerificationPurpose entity, CancellationToken ct) => entity);

        // Act
        VerificationPurposeDto result = await _service.CreateIfNotExistsAsync(id, request);

        // Assert
        capturedEntity.Should().NotBeNull();
        capturedEntity!.Id.Should().Be(id);
        result.Id.Should().Be(id);
    }

    [Fact]
    public async Task CreateIfNotExistsAsync_WithDifferentCodes_ShouldCreateSeparateEntities()
    {
        // Arrange
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();

        var request1 = new CreateVerificationPurposeRequest(
            Code: "EMAIL_VERIFICATION",
            Name: "Email Verification",
            Description: "Verify email"
        );

        var request2 = new CreateVerificationPurposeRequest(
            Code: "PASSWORD_RESET",
            Name: "Password Reset",
            Description: "Reset password"
        );

        _readRepositoryMock
            .Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<VerificationPurpose, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((VerificationPurpose?)null);

        var capturedEntities = new List<VerificationPurpose>();
        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<VerificationPurpose>(), It.IsAny<CancellationToken>()))
            .Callback<VerificationPurpose, CancellationToken>((entity, ct) => capturedEntities.Add(entity))
            .ReturnsAsync((VerificationPurpose entity, CancellationToken ct) => entity);

        // Act
        VerificationPurposeDto result1 = await _service.CreateIfNotExistsAsync(id1, request1);
        VerificationPurposeDto result2 = await _service.CreateIfNotExistsAsync(id2, request2);

        // Assert
        result1.Code.Should().Be("EMAIL_VERIFICATION");
        result2.Code.Should().Be("PASSWORD_RESET");

        capturedEntities.Should().HaveCount(2);
        capturedEntities[0].Id.Should().Be(id1);
        capturedEntities[1].Id.Should().Be(id2);
    }

    #endregion
}
