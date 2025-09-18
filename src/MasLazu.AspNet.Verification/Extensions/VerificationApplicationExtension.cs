using Microsoft.Extensions.DependencyInjection;
using MasLazu.AspNet.Verification.Services;
using MasLazu.AspNet.Verification.Abstraction.Interfaces;

namespace MasLazu.AspNet.Verification.Extensions;

public static class VerificationApplicationExtension
{
    public static IServiceCollection AddVerificationApplication(this IServiceCollection services)
    {
        services.AddScoped<IVerificationService, VerificationService>();
        services.AddScoped<IVerificationPurposeService, VerificationPurposeService>();
        services.AddVerificationApplicationUtils();
        services.AddVerificationApplicationValidators();

        return services;
    }
}
