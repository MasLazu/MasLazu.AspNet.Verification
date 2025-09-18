using System.Linq.Expressions;
using MasLazu.AspNet.Framework.Application.Interfaces;
using MasLazu.AspNet.Verification.Domain.Entities;

namespace MasLazu.AspNet.Verification.Utils;

public class VerificationPurposePropertyMap : IEntityPropertyMap<VerificationPurpose>
{
    private readonly Dictionary<string, Expression<Func<VerificationPurpose, object>>> _map =
        new(StringComparer.OrdinalIgnoreCase)
        {
            { "id", vp => vp.Id },
            { "code", vp => vp.Code },
            { "name", vp => vp.Name },
            { "description", vp => vp.Description },
            { "isActive", vp => vp.IsActive },
            { "createdAt", vp => vp.CreatedAt },
            { "updatedAt", vp => vp.UpdatedAt }
        };

    public Expression<Func<VerificationPurpose, object>> Get(string property)
    {
        if (_map.TryGetValue(property, out Expression<Func<VerificationPurpose, object>>? expr))
        {
            return expr;
        }

        throw new ArgumentException($"Unknown property: {property}");
    }
}
