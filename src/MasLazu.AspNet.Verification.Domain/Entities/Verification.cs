using System;
using MasLazu.AspNet.Framework.Domain.Entities;
using MasLazu.AspNet.Verification.Abstraction.Enums;

namespace MasLazu.AspNet.Verification.Domain.Entities;

public class Verification : BaseEntity
{
    public Guid UserId { get; set; }
    public VerificationChannel Channel { get; set; }
    public string Destination { get; set; } = string.Empty; // Email or phone number
    public string VerificationCode { get; set; } = string.Empty;
    public string VerificationPurposeCode { get; set; } = string.Empty;
    public VerificationStatus Status { get; set; } = VerificationStatus.Pending;
    public int AttemptCount { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
    public DateTimeOffset? VerifiedAt { get; set; }

    public VerificationPurpose? VerificationPurpose { get; set; }
}
