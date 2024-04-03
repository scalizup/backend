using System.Reflection;
using Application.Common.PreProcessors;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddOpenRequestPreProcessor(typeof(AuthorizationRequestPreProcessor<>));
            cfg.AddOpenRequestPreProcessor(typeof(ValidationRequestPreProcessor<>));
        });

        return services;
    }
}