using FluentValidation;
using MasLazu.AspNet.Verification.Abstraction.Interfaces;
using MasLazu.AspNet.Verification.Abstraction.Models;
using MasLazu.AspNet.Verification.Domain.Entities;
using MasLazu.AspNet.Framework.Application.Interfaces;
using MasLazu.AspNet.Framework.Application.Services;
using MasLazu.AspNet.Verification.Abstraction.Enums;
using MasLazu.AspNet.EmailSender.Abstraction.Interfaces;
using MasLazu.AspNet.EmailSender.Abstraction.Models;
using Mapster;
using MassTransit;
using MasLazu.AspNet.Verification.Abstraction.Events;

namespace MasLazu.AspNet.Verification.Services;

public class VerificationService : CrudService<Domain.Entities.Verification, VerificationDto, CreateVerificationRequest, UpdateVerificationRequest>, IVerificationService
{
    private readonly IEmailSender _emailSender;
    private readonly IHtmlRenderer _htmlRenderer;
    private readonly IPublishEndpoint _publishEndpoint;

    public VerificationService(
        IRepository<Domain.Entities.Verification> repository,
        IReadRepository<Domain.Entities.Verification> readRepository,
        IUnitOfWork unitOfWork,
        IEntityPropertyMap<Domain.Entities.Verification> propertyMap,
        IPaginationValidator<Domain.Entities.Verification> paginationValidator,
        ICursorPaginationValidator<Domain.Entities.Verification> cursorPaginationValidator,
        IEmailSender emailSender,
        IHtmlRenderer htmlRenderer,
        IPublishEndpoint publishEndpoint,
        IValidator<CreateVerificationRequest>? createValidator = null,
        IValidator<UpdateVerificationRequest>? updateValidator = null)
        : base(repository, readRepository, unitOfWork, propertyMap, paginationValidator, cursorPaginationValidator, createValidator, updateValidator)
    {
        _emailSender = emailSender;
        _htmlRenderer = htmlRenderer;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<VerificationDto?> GetByCodeAsync(Guid userId, string code, CancellationToken ct = default)
    {
        return (await ReadRepository.FirstOrDefaultAsync(v => v.VerificationCode == code, ct))?.Adapt<VerificationDto>();
    }

    public async Task<bool> IsCodeValidAsync(Guid userId, string code, CancellationToken ct = default)
    {
        Domain.Entities.Verification? verification = await ReadRepository.FirstOrDefaultAsync(v =>
            v.VerificationCode == code &&
            v.Status == VerificationStatus.Pending &&
            v.ExpiresAt > DateTimeOffset.UtcNow, ct);

        return verification is not null;
    }

    public async Task<VerificationDto> VerifyAsync(string code, CancellationToken ct = default)
    {
        Domain.Entities.Verification verification = await ReadRepository.FirstOrDefaultAsync(v =>
            v.VerificationCode == code &&
            v.Status == VerificationStatus.Pending &&
            v.ExpiresAt > DateTimeOffset.UtcNow, ct) ?? throw new InvalidOperationException("Invalid or expired verification code");

        verification.Status = VerificationStatus.Verified;
        verification.VerifiedAt = DateTimeOffset.UtcNow;
        verification.AttemptCount++;

        await Repository.UpdateAsync(verification, ct);
        await UnitOfWork.SaveChangesAsync(ct);

        // Publish verification completed event
        var verificationEvent = new VerificationCompletedEvent
        {
            VerificationId = verification.Id,
            UserId = verification.UserId,
            Email = verification.Destination,
            PurposeCode = verification.VerificationPurposeCode,
            CompletedAt = DateTime.UtcNow,
            IsSuccessful = true
        };

        await _publishEndpoint.Publish(verificationEvent, ct);

        return verification.Adapt<VerificationDto>();
    }

    public async Task<VerificationDto> CreateVerificationAsync(Guid userId, CreateVerificationRequest request, CancellationToken ct = default)
    {
        string code = GenerateVerificationCode();

        var verification = new Domain.Entities.Verification
        {
            UserId = request.UserId,
            Channel = request.Channel,
            Destination = request.Destination,
            VerificationCode = code,
            VerificationPurposeCode = request.VerificationPurposeCode,
            Status = VerificationStatus.Pending,
            ExpiresAt = request.ExpiresAt ?? DateTimeOffset.UtcNow.AddMinutes(10),
            AttemptCount = 0
        };

        await Repository.AddAsync(verification, ct);
        await UnitOfWork.SaveChangesAsync(ct);

        return verification.Adapt<VerificationDto>();
    }

    public async Task<VerificationDto> SendVerificationAsync(Guid userId, SendVerificationRequest request, CancellationToken ct = default)
    {
        var createRequest = new CreateVerificationRequest(
            UserId: request.UserId,
            Channel: VerificationChannel.Email,
            Destination: request.Destination,
            VerificationPurposeCode: request.PurposeCode,
            ExpiresAt: request.ExpiresAt
        );

        VerificationDto verification = await CreateVerificationAsync(userId, createRequest, ct);

        await SendVerificationCodeAsync(verification, ct);

        return verification;
    }

    private async Task SendVerificationCodeAsync(VerificationDto verification, CancellationToken ct = default)
    {
        await SendEmailVerificationAsync(verification, ct);
    }

    private async Task SendEmailVerificationAsync(VerificationDto verification, CancellationToken ct = default)
    {
        string verificationCode = verification.VerificationCode;

        EmailMessage email = new EmailMessageBuilder()
            .To(verification.Destination)
            .Subject("🔐 Verify Your Account")
            .RenderOptions(new EmailRenderOptions
            {
                Theme = "VerificationCode",
                PrimaryColor = "#28a745"
            })
            .Model(new
            {
                VerificationCode = verificationCode,
                // UserName = "User", // Replace with actual user name if available
                ExpiryMinutes = (int)(verification.ExpiresAt - DateTimeOffset.UtcNow).TotalMinutes
            })
            .Build();

        await _emailSender.SendEmailAsync(email, _htmlRenderer);
    }

    private string GenerateVerificationCode()
    {
        var random = new Random();
        return random.Next(100000, 999999).ToString();
    }
}
