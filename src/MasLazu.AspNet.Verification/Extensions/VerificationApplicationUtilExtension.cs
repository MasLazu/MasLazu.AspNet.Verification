using Microsoft.Extensions.DependencyInjection;
using MasLazu.AspNet.Verification.Utils;
using MasLazu.AspNet.Verification.Domain.Entities;
using MasLazu.AspNet.Framework.Application.Interfaces;
using MasLazu.AspNet.Framework.Application.Utils;

namespace MasLazu.AspNet.Verification.Extensions;

public static class VerificationApplicationUtilExtension
{
    public static IServiceCollection AddVerificationApplicationUtils(this IServiceCollection services)
    {
        RegisterPropertyMapsAndExpressionBuilders(services);

        return services;
    }

    private static void RegisterPropertyMapsAndExpressionBuilders(IServiceCollection services)
    {
        var entityPropertyMapPairs = new (Type entityType, Type propertyMapType)[]
        {
            (typeof(Domain.Entities.Verification), typeof(VerificationEntityPropertyMap)),
            (typeof(VerificationPurpose), typeof(VerificationPurposeEntityPropertyMap))
        };

        foreach ((Type entityType, Type propertyMapType) in entityPropertyMapPairs)
        {
            Type propertyMapInterfaceType = typeof(IEntityPropertyMap<>).MakeGenericType(entityType);
            services.AddSingleton(propertyMapInterfaceType, propertyMapType);

            Type expressionBuilderType = typeof(ExpressionBuilder<>).MakeGenericType(entityType);
            services.AddScoped(expressionBuilderType);
        }
    }
}
