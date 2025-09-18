using FastEndpoints;
using Microsoft.AspNetCore.Http;
using MasLazu.AspNet.Framework.Endpoint.EndpointGroups;

namespace MasLazu.AspNet.Verification.Endpoint.EndpointGroups;

public class VerificationEndpointGroup : SubGroup<V1EndpointGroup>
{
    public VerificationEndpointGroup()
    {
        Configure("verification", ep => ep.Description(x => x.WithTags("Verification")));
    }
}
