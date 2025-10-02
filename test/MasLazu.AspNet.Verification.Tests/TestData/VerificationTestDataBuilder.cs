using Bogus;
using MasLazu.AspNet.Verification.Abstraction.Enums;
using MasLazu.AspNet.Verification.Abstraction.Models;
using MasLazu.AspNet.Verification.Domain.Entities;

namespace MasLazu.AspNet.Verification.Tests.TestData;

/// <summary>
/// Test data builder for creating test verification entities and requests
/// </summary>
public static class VerificationTestDataBuilder
{
    private static readonly Faker Faker = new();

    public static Domain.Entities.Verification CreateValidVerification()
    {
        return new Domain.Entities.Verification
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Channel = VerificationChannel.Email,
            Destination = Faker.Internet.Email(),
            VerificationCode = Faker.Random.Number(100000, 999999).ToString(),
            VerificationPurposeCode = "REGISTRATION",
            Status = VerificationStatus.Pending,
            AttemptCount = 0,
            ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(10),
            VerifiedAt = null,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = null
        };
    }

    public static Domain.Entities.Verification CreateExpiredVerification()
    {
        Domain.Entities.Verification verification = CreateValidVerification();
        verification.ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(-10);
        return verification;
    }

    public static Domain.Entities.Verification CreateVerifiedVerification()
    {
        Domain.Entities.Verification verification = CreateValidVerification();
        verification.Status = VerificationStatus.Verified;
        verification.VerifiedAt = DateTimeOffset.UtcNow.AddMinutes(-5);
        verification.AttemptCount = 1;
        return verification;
    }

    public static Domain.Entities.Verification CreateVerificationWithStatus(VerificationStatus status)
    {
        Domain.Entities.Verification verification = CreateValidVerification();
        verification.Status = status;
        if (status == VerificationStatus.Verified)
        {
            verification.VerifiedAt = DateTimeOffset.UtcNow;
        }
        return verification;
    }

    public static CreateVerificationRequest CreateValidRequest()
    {
        return new CreateVerificationRequest(
            UserId: Guid.NewGuid(),
            Channel: VerificationChannel.Email,
            Destination: Faker.Internet.Email(),
            VerificationPurposeCode: "REGISTRATION",
            ExpiresAt: DateTimeOffset.UtcNow.AddMinutes(15)
        );
    }

    public static SendVerificationRequest CreateValidSendRequest()
    {
        return new SendVerificationRequest(
            UserId: Guid.NewGuid(),
            Destination: Faker.Internet.Email(),
            PurposeCode: "REGISTRATION",
            ExpiresAt: DateTimeOffset.UtcNow.AddMinutes(15)
        );
    }

    public static UpdateVerificationRequest CreateValidUpdateRequest(Guid id)
    {
        return new UpdateVerificationRequest(
            Id: id,
            Channel: VerificationChannel.Email,
            Destination: Faker.Internet.Email(),
            VerificationPurposeCode: "PASSWORD_RESET",
            Status: VerificationStatus.Pending,
            ExpiresAt: DateTimeOffset.UtcNow.AddMinutes(20)
        );
    }

    public static VerificationPurpose CreateValidPurpose()
    {
        return new VerificationPurpose
        {
            Id = Guid.NewGuid(),
            Code = "REGISTRATION",
            Name = "Registration Verification",
            Description = "Email verification for new user registration",
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = null
        };
    }

    public static CreateVerificationPurposeRequest CreateValidPurposeRequest()
    {
        return new CreateVerificationPurposeRequest(
            Code: "REGISTRATION",
            Name: "Registration Verification",
            Description: "Email verification for new user registration",
            IsActive: true
        );
    }
}
