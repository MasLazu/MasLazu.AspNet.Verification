using System.Linq.Expressions;
using MasLazu.AspNet.Framework.Application.Interfaces;
using MasLazu.AspNet.Verification.Domain.Entities;

namespace MasLazu.AspNet.Verification.Utils;

public class VerificationPropertyMap : IEntityPropertyMap<Domain.Entities.Verification>
{
    private readonly Dictionary<string, Expression<Func<Domain.Entities.Verification, object>>> _map =
        new(StringComparer.OrdinalIgnoreCase)
        {
            { "id", v => v.Id },
            { "userId", v => v.UserId },
            { "channel", v => v.Channel },
            { "destination", v => v.Destination },
            { "verificationCode", v => v.VerificationCode },
            { "verificationPurposeCode", v => v.VerificationPurposeCode },
            { "status", v => v.Status },
            { "attemptCount", v => v.AttemptCount },
            { "expiresAt", v => v.ExpiresAt },
            { "verifiedAt", v => v.VerifiedAt },
            { "createdAt", v => v.CreatedAt },
            { "updatedAt", v => v.UpdatedAt }
        };

    public Expression<Func<Domain.Entities.Verification, object>> Get(string property)
    {
        if (_map.TryGetValue(property, out Expression<Func<Domain.Entities.Verification, object>> expr))
        {
            return expr;
        }

        throw new ArgumentException($"Unknown property: {property}");
    }
}
