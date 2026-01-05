using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MiniAuth.Application.Auth.Commands.Register;
using MiniAuth.Application.Common.Behaviors;

namespace MiniAuth.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(cfg =>
        {
            cfg.LicenseKey = configuration["MediatrLicenseKey"];
            cfg.RegisterServicesFromAssembly(typeof(RegisterCommandHandler).Assembly);
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });
        services.AddValidatorsFromAssembly(typeof(RegisterCommandHandler).Assembly);
        return services;
    }
}