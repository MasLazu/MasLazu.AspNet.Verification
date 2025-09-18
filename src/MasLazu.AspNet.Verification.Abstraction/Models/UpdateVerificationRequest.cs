using MasLazu.AspNet.Framework.Application.Models;
using MasLazu.AspNet.Verification.Abstraction.Enums;

namespace MasLazu.AspNet.Verification.Abstraction.Models;

public record UpdateVerificationRequest(
    Guid Id,
    VerificationChannel? Channel = null,
    string? Destination = null,
    string? VerificationPurposeCode = null,
    VerificationStatus? Status = null,
    DateTimeOffset? ExpiresAt = null
) : BaseUpdateRequest(Id);
