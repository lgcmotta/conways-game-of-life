using HashidsNet;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Conways.GameOfLife.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHashIds(this IServiceCollection services, IConfiguration configuration)
    {
        var salt = configuration.GetValue<string>("HashIds:Salt");

        var minHashLength = configuration.GetValue<int>("HashIds:MinHashLength");
        
        ArgumentException.ThrowIfNullOrWhiteSpace(salt);

        services.AddSingleton<IHashids>(new Hashids(salt, minHashLength));
        
        return services;
    }
}