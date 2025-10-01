using MasLazu.AspNet.Framework.Application.Interfaces;
using MasLazu.AspNet.Framework.EfCore.Repositories;
using MasLazu.AspNet.Verification.EfCore.Data;
using Microsoft.Extensions.DependencyInjection;
using MasLazu.AspNet.Verification.Domain.Entities;

namespace MasLazu.AspNet.Verification.EfCore.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddVerificationEntityFrameworkCore(this IServiceCollection services)
    {
        services.AddScoped<IRepository<Domain.Entities.Verification>, Repository<Domain.Entities.Verification, VerificationDbContext>>();
        services.AddScoped<IReadRepository<Domain.Entities.Verification>, ReadRepository<Domain.Entities.Verification, VerificationReadDbContext>>();
        services.AddScoped<IRepository<VerificationPurpose>, Repository<VerificationPurpose, VerificationDbContext>>();
        services.AddScoped<IReadRepository<VerificationPurpose>, ReadRepository<VerificationPurpose, VerificationReadDbContext>>();

        return services;
    }
}
