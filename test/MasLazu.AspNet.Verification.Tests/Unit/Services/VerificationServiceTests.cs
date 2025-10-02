using FluentAssertions;
using MasLazu.AspNet.EmailSender.Abstraction.Interfaces;
using MasLazu.AspNet.EmailSender.Abstraction.Models;
using MasLazu.AspNet.Framework.Application.Interfaces;
using MasLazu.AspNet.Verification.Abstraction.Enums;
using MasLazu.AspNet.Verification.Abstraction.Events;
using MasLazu.AspNet.Verification.Abstraction.Models;
using MasLazu.AspNet.Verification.Domain.Entities;
using MasLazu.AspNet.Verification.Services;
using MassTransit;
using Moq;
using System.Linq.Expressions;
using Xunit;
using TestMockFactory = MasLazu.AspNet.Verification.Tests.Fixtures.MockFactory;

namespace MasLazu.AspNet.Verification.Tests.Unit.Services;

public class VerificationServiceTests
{
    private readonly Mock<IRepository<Domain.Entities.Verification>> _repositoryMock;
    private readonly Mock<IReadRepository<Domain.Entities.Verification>> _readRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IEmailSender> _emailSenderMock;
    private readonly IHtmlRenderer _htmlRendererMock;
    private readonly Mock<IPublishEndpoint> _publishEndpointMock;
    private readonly VerificationService _service;

    public VerificationServiceTests()
    {
        _repositoryMock = TestMockFactory.CreateRepositoryMock<Domain.Entities.Verification>();
        _readRepositoryMock = TestMockFactory.CreateReadRepositoryMock<Domain.Entities.Verification>();
        _unitOfWorkMock = TestMockFactory.CreateUnitOfWorkMock();
        _emailSenderMock = TestMockFactory.CreateEmailSenderMock();
        _htmlRendererMock = Mock.Of<IHtmlRenderer>();
        _publishEndpointMock = TestMockFactory.CreatePublishEndpointMock();

        _service = new VerificationService(
            _repositoryMock.Object,
            _readRepositoryMock.Object,
            _unitOfWorkMock.Object,
            TestMockFactory.CreatePropertyMapMock<Domain.Entities.Verification>().Object,
            TestMockFactory.CreatePaginationValidatorMock<Domain.Entities.Verification>().Object,
            TestMockFactory.CreateCursorPaginationValidatorMock<Domain.Entities.Verification>().Object,
            _emailSenderMock.Object,
            _htmlRendererMock,
            _publishEndpointMock.Object
        );
    }

    #region GetByCodeAsync Tests

    [Fact]
    public async Task GetByCodeAsync_WithExistingCode_ShouldReturnVerificationDto()
    {
        // Arrange
        var userId = Guid.NewGuid();
        string code = "123456";
        var verification = new Domain.Entities.Verification
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            VerificationCode = code,
            Channel = VerificationChannel.Email,
            Destination = "test@example.com",
            VerificationPurposeCode = "EMAIL_VERIFICATION",
            Status = VerificationStatus.Pending,
            ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(10)
        };

        _readRepositoryMock
            .Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<Domain.Entities.Verification, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(verification);

        // Act
        VerificationDto? result = await _service.GetByCodeAsync(userId, code);

        // Assert
        result.Should().NotBeNull();
        result!.VerificationCode.Should().Be(code);
        result.UserId.Should().Be(userId);
    }

    [Fact]
    public async Task GetByCodeAsync_WithNonExistingCode_ShouldReturnNull()
    {
        // Arrange
        var userId = Guid.NewGuid();
        string code = "999999";

        _readRepositoryMock
            .Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<Domain.Entities.Verification, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Verification?)null);

        // Act
        VerificationDto? result = await _service.GetByCodeAsync(userId, code);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region IsCodeValidAsync Tests

    [Fact]
    public async Task IsCodeValidAsync_WithValidCode_ShouldReturnTrue()
    {
        // Arrange
        var userId = Guid.NewGuid();
        string code = "123456";
        var verification = new Domain.Entities.Verification
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            VerificationCode = code,
            Channel = VerificationChannel.Email,
            Destination = "test@example.com",
            VerificationPurposeCode = "EMAIL_VERIFICATION",
            Status = VerificationStatus.Pending,
            ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(10)
        };

        _readRepositoryMock
            .Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<Domain.Entities.Verification, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(verification);

        // Act
        bool result = await _service.IsCodeValidAsync(userId, code);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsCodeValidAsync_WithExpiredCode_ShouldReturnFalse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        string code = "123456";

        _readRepositoryMock
            .Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<Domain.Entities.Verification, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Verification?)null);

        // Act
        bool result = await _service.IsCodeValidAsync(userId, code);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsCodeValidAsync_WithNonExistingCode_ShouldReturnFalse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        string code = "999999";

        _readRepositoryMock
            .Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<Domain.Entities.Verification, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Verification?)null);

        // Act
        bool result = await _service.IsCodeValidAsync(userId, code);

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region VerifyAsync Tests

    [Fact]
    public async Task VerifyAsync_WithValidCode_ShouldUpdateStatusAndPublishEvent()
    {
        // Arrange
        string code = "123456";
        var userId = Guid.NewGuid();
        var verification = new Domain.Entities.Verification
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            VerificationCode = code,
            Channel = VerificationChannel.Email,
            Destination = "test@example.com",
            VerificationPurposeCode = "EMAIL_VERIFICATION",
            Status = VerificationStatus.Pending,
            ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(10),
            AttemptCount = 0
        };

        _readRepositoryMock
            .Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<Domain.Entities.Verification, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(verification);

        // Act
        VerificationDto result = await _service.VerifyAsync(code);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(VerificationStatus.Verified);
        result.VerifiedAt.Should().NotBeNull();
        result.AttemptCount.Should().Be(1);

        _repositoryMock.Verify(r => r.UpdateAsync(
            It.Is<Domain.Entities.Verification>(v => v.Status == VerificationStatus.Verified),
            It.IsAny<CancellationToken>()), Times.Once);

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _publishEndpointMock.Verify(p => p.Publish(
            It.Is<VerificationCompletedEvent>(e =>
                e.VerificationId == verification.Id &&
                e.UserId == userId &&
                e.IsSuccessful),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task VerifyAsync_WithInvalidCode_ShouldThrowInvalidOperationException()
    {
        // Arrange
        string code = "999999";

        _readRepositoryMock
            .Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<Domain.Entities.Verification, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Verification?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.VerifyAsync(code));
    }

    [Fact]
    public async Task VerifyAsync_WithExpiredCode_ShouldThrowInvalidOperationException()
    {
        // Arrange
        string code = "123456";

        _readRepositoryMock
            .Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<Domain.Entities.Verification, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Verification?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.VerifyAsync(code));
    }

    #endregion

    #region CreateVerificationAsync Tests

    [Fact]
    public async Task CreateVerificationAsync_WithValidRequest_ShouldCreateVerification()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new CreateVerificationRequest(
            UserId: userId,
            Channel: VerificationChannel.Email,
            Destination: "test@example.com",
            VerificationPurposeCode: "EMAIL_VERIFICATION",
            ExpiresAt: DateTimeOffset.UtcNow.AddMinutes(30)
        );

        Domain.Entities.Verification? capturedVerification = null;
        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Domain.Entities.Verification>(), It.IsAny<CancellationToken>()))
            .Callback<Domain.Entities.Verification, CancellationToken>((v, ct) => capturedVerification = v)
            .ReturnsAsync((Domain.Entities.Verification v, CancellationToken ct) => v);

        // Act
        VerificationDto result = await _service.CreateVerificationAsync(userId, request);

        // Assert
        result.Should().NotBeNull();
        result.UserId.Should().Be(userId);
        result.Channel.Should().Be(VerificationChannel.Email);
        result.Destination.Should().Be("test@example.com");
        result.VerificationCode.Should().NotBeNullOrEmpty();
        result.VerificationCode.Should().HaveLength(6);
        result.Status.Should().Be(VerificationStatus.Pending);

        capturedVerification.Should().NotBeNull();
        capturedVerification!.VerificationCode.Should().MatchRegex(@"^\d{6}$");

        _repositoryMock.Verify(r => r.AddAsync(
            It.IsAny<Domain.Entities.Verification>(),
            It.IsAny<CancellationToken>()), Times.Once);

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateVerificationAsync_WithoutExpiresAt_ShouldUseDefaultExpiration()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new CreateVerificationRequest(
            UserId: userId,
            Channel: VerificationChannel.Email,
            Destination: "test@example.com",
            VerificationPurposeCode: "EMAIL_VERIFICATION",
            ExpiresAt: null
        );

        Domain.Entities.Verification? capturedVerification = null;
        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Domain.Entities.Verification>(), It.IsAny<CancellationToken>()))
            .Callback<Domain.Entities.Verification, CancellationToken>((v, ct) => capturedVerification = v)
            .ReturnsAsync((Domain.Entities.Verification v, CancellationToken ct) => v);

        // Act
        VerificationDto result = await _service.CreateVerificationAsync(userId, request);

        // Assert
        result.Should().NotBeNull();
        capturedVerification.Should().NotBeNull();
        capturedVerification!.ExpiresAt.Should().BeCloseTo(
            DateTimeOffset.UtcNow.AddMinutes(10),
            TimeSpan.FromSeconds(5));
    }

    #endregion

    #region SendVerificationAsync Tests

    [Fact]
    public async Task SendVerificationAsync_WithValidRequest_ShouldCreateAndSendVerification()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new SendVerificationRequest(
            UserId: userId,
            Destination: "test@example.com",
            PurposeCode: "EMAIL_VERIFICATION",
            ExpiresAt: DateTimeOffset.UtcNow.AddMinutes(30)
        );

        Domain.Entities.Verification? capturedVerification = null;
        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Domain.Entities.Verification>(), It.IsAny<CancellationToken>()))
            .Callback<Domain.Entities.Verification, CancellationToken>((v, ct) => capturedVerification = v)
            .ReturnsAsync((Domain.Entities.Verification v, CancellationToken ct) => v);

        // Act
        VerificationDto result = await _service.SendVerificationAsync(userId, request);

        // Assert
        result.Should().NotBeNull();
        result.UserId.Should().Be(userId);
        result.Destination.Should().Be("test@example.com");
        result.VerificationCode.Should().NotBeNullOrEmpty();

        _emailSenderMock.Verify(e => e.SendEmailAsync(
            It.Is<EmailMessage>(m =>
                m.To.Any(a => a.Email == "test@example.com") &&
                m.Subject.Contains("Verify")),
            It.IsAny<IHtmlRenderer>()), Times.Once);

        _repositoryMock.Verify(r => r.AddAsync(
            It.IsAny<Domain.Entities.Verification>(),
            It.IsAny<CancellationToken>()), Times.Once);

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SendVerificationAsync_ShouldSendEmailWithCorrectFormat()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new SendVerificationRequest(
            UserId: userId,
            Destination: "test@example.com",
            PurposeCode: "EMAIL_VERIFICATION"
        );

        EmailMessage? capturedEmail = null;
        _emailSenderMock
            .Setup(e => e.SendEmailAsync(It.IsAny<EmailMessage>(), It.IsAny<IHtmlRenderer>()))
            .Callback<EmailMessage, IHtmlRenderer>((email, renderer) => capturedEmail = email)
            .Returns(Task.CompletedTask);

        // Act
        await _service.SendVerificationAsync(userId, request);

        // Assert
        capturedEmail.Should().NotBeNull();
        capturedEmail!.To.Should().Contain(a => a.Email == "test@example.com");
        capturedEmail.Subject.Should().Contain("Verify");
        capturedEmail.RenderOptions.Should().NotBeNull();
        capturedEmail.RenderOptions!.Theme.Should().Be("VerificationCode");
    }

    #endregion
}
