using System.Diagnostics.CodeAnalysis;
using Conways.GameOfLife.Infrastructure.PostgreSQL.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Conways.GameOfLife.Infrastructure.PostgreSQL.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBoardDbContexts(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(nameof(BoardDbContext));
        
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);
        
        var retries = configuration.GetValue<int>("PostgresSettings:RetryCount");

        services.AddBoardDbContext<BoardDbContext>(connectionString, retries);
        
        services.AddBoardDbContext<BoardDbContextReadOnly>(connectionString, retries, useInterceptors: false);
        
        return services;
    }

    private static IServiceCollection AddBoardDbContext<TDbContext>(
        this IServiceCollection services,
        string connectionString,
        int retries,
        bool useInterceptors = true)
        where TDbContext : DbContext
    {
        services.AddDbContext<TDbContext>((provider, builder) =>
        {
            builder.UseNpgsql(connectionString, pgsql =>
            {
                pgsql.EnableRetryOnFailure(retries);
            });

            if (!useInterceptors) return;
            
            var interceptors = InterceptorsAssemblyScanner.Scan(provider, typeof(BoardDbContext).Assembly);

            builder.AddInterceptors(interceptors);
        });

        return services;
    }
}