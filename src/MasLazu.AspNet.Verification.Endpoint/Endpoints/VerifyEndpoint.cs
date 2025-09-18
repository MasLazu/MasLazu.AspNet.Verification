using FastEndpoints;
using MasLazu.AspNet.Framework.Endpoint.Endpoints;
using MasLazu.AspNet.Verification.Abstraction.Interfaces;
using MasLazu.AspNet.Verification.Abstraction.Models;
using MasLazu.AspNet.Verification.Endpoint.Models;
using MasLazu.AspNet.Verification.Endpoint.EndpointGroups;

namespace MasLazu.AspNet.Verification.Endpoint.Endpoints;

public class VerifyEndpoint : BaseEndpoint<VerifyRequest, VerificationDto>
{
    public IVerificationService VerificationService { get; set; }

    public override void ConfigureEndpoint()
    {
        Post("/verify");
        Group<VerificationEndpointGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(VerifyRequest req, CancellationToken ct)
    {
        VerificationDto result = await VerificationService.VerifyAsync(req.Code, ct);
        await SendOkResponseAsync(result, "Verification successful", ct);
    }
}
