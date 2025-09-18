using MasLazu.AspNet.Framework.Domain.Entities;

namespace MasLazu.AspNet.Verification.Domain.Entities;

public class VerificationPurpose : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public ICollection<Verification> Verifications { get; set; } = [];
}
