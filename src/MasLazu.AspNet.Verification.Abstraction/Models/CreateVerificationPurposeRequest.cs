namespace MasLazu.AspNet.Verification.Abstraction.Models;

public record CreateVerificationPurposeRequest(
    string Code,
    string Name,
    string? Description,
    bool IsActive = true
);
