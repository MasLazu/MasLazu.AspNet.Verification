using MasLazu.AspNet.Verification.Abstraction.Enums;

namespace MasLazu.AspNet.Verification.Abstraction.Models;

public record CreateVerificationRequest(
    Guid UserId,
    VerificationChannel Channel,
    string Destination,
    string VerificationPurposeCode,
    DateTimeOffset? ExpiresAt = null
);
