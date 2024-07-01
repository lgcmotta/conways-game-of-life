using System.Diagnostics.CodeAnalysis;
using Conways.GameOfLife.API.Behaviors;
using FluentValidation;

namespace Conways.GameOfLife.API.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCQRS(this IServiceCollection services)
    {
        return services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssemblyContaining<Program>();
            config.AddOpenBehavior(typeof(LoggingBehavior<,>));
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
            config.Lifetime = ServiceLifetime.Scoped;
        });
    }

    public static IServiceCollection AddFluentValidators(this IServiceCollection services)
    {
        return services.AddValidatorsFromAssemblyContaining<Program>();
    }
}