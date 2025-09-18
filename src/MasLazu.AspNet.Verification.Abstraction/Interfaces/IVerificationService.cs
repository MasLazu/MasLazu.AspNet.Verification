using MasLazu.AspNet.Verification.Abstraction.Models;

namespace MasLazu.AspNet.Verification.Abstraction.Interfaces;

public interface IVerificationService
{
    Task<VerificationDto?> GetByCodeAsync(Guid userId, string code, CancellationToken ct = default);
    Task<bool> IsCodeValidAsync(Guid userId, string code, CancellationToken ct = default);
    Task<VerificationDto> VerifyAsync(string code, CancellationToken ct = default);
    Task<VerificationDto> CreateVerificationAsync(Guid userId, CreateVerificationRequest request, CancellationToken ct = default);
    Task<VerificationDto> SendVerificationAsync(Guid userId, SendVerificationRequest request, CancellationToken ct = default);
}
