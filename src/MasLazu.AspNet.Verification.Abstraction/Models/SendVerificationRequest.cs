namespace MasLazu.AspNet.Verification.Abstraction.Models;

public record SendVerificationRequest(
    Guid UserId,
    string Destination,
    string PurposeCode,
    DateTimeOffset? ExpiresAt = null
);
