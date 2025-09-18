using Microsoft.Extensions.DependencyInjection;
using MasLazu.AspNet.Verification.Validators;
using FluentValidation;
using MasLazu.AspNet.Verification.Abstraction.Models;

namespace MasLazu.AspNet.Verification.Extensions;

public static class VerificationApplicationValidatorExtension
{
    public static IServiceCollection AddVerificationApplicationValidators(this IServiceCollection services)
    {
        services.AddScoped<IValidator<CreateVerificationRequest>, CreateVerificationRequestValidator>();
        services.AddScoped<IValidator<UpdateVerificationRequest>, UpdateVerificationRequestValidator>();
        services.AddScoped<IValidator<SendVerificationRequest>, SendVerificationRequestValidator>();
        services.AddScoped<IValidator<CreateVerificationPurposeRequest>, CreateVerificationPurposeRequestValidator>();
        services.AddScoped<IValidator<UpdateVerificationPurposeRequest>, UpdateVerificationPurposeRequestValidator>();
        services.AddScoped<IValidator<string>, VerifyCodeRequestValidator>();

        return services;
    }
}
