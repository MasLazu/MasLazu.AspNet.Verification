using MasLazu.AspNet.Framework.Application.Models;
using MasLazu.AspNet.Verification.Abstraction.Enums;

namespace MasLazu.AspNet.Verification.Abstraction.Models;

public record VerificationDto(
    Guid Id,
    Guid UserId,
    VerificationChannel Channel,
    string Destination,
    string VerificationCode,
    string VerificationPurposeCode,
    VerificationStatus Status,
    int AttemptCount,
    DateTimeOffset ExpiresAt,
    DateTimeOffset? VerifiedAt,
    VerificationPurposeDto? VerificationPurpose,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt
) : BaseDto(Id, CreatedAt, UpdatedAt);
