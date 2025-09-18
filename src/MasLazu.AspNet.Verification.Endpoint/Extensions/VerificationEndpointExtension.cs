using Microsoft.Extensions.DependencyInjection;

namespace MasLazu.AspNet.Verification.Endpoint.Extensions;

public static class VerificationEndpointExtension
{
    public static IServiceCollection AddVerificationEndpoints(this IServiceCollection services)
    {
        return services;
    }
}
