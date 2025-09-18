using Microsoft.Extensions.DependencyInjection;

namespace MasLazu.AspNet.Verification.EfCore.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddVerificationEntityFrameworkCore(this IServiceCollection services)
    {
        return services;
    }
}
