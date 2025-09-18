using MasLazu.AspNet.Framework.Application.Models;

namespace MasLazu.AspNet.Verification.Abstraction.Models;

public record UpdateVerificationPurposeRequest(
    Guid Id,
    string? Code = null,
    string? Name = null,
    string? Description = null,
    bool? IsActive = null
) : BaseUpdateRequest(Id);
