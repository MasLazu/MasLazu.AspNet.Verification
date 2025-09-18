using MasLazu.AspNet.Framework.Application.Models;

namespace MasLazu.AspNet.Verification.Abstraction.Models;

public record VerificationPurposeDto(
    Guid Id,
    string Code,
    string Name,
    string? Description,
    bool IsActive,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt
) : BaseDto(Id, CreatedAt, UpdatedAt);
