using MasLazu.AspNet.Verification.Abstraction.Models;

namespace MasLazu.AspNet.Verification.Abstraction.Interfaces;

public interface IVerificationPurposeService
{
    Task<VerificationPurposeDto> CreateIfNotExistsAsync(Guid id, CreateVerificationPurposeRequest createRequest, CancellationToken ct = default);
}
