using Conways.GameOfLife.Infrastructure.PostgresSQL.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Conways.GameOfLife.Infrastructure.PostgresSQL.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBoardDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(nameof(BoardDbContext));

        var retries = configuration.GetValue<int>("PostgresSettings:RetryCount");

        services.AddDbContext<BoardDbContext>((provider, builder) =>
        {
            builder.UseNpgsql(connectionString, pgsql =>
            {
                pgsql.EnableRetryOnFailure(retries);
            });

            var interceptors = InterceptorsAssemblyScanner.Scan(provider, typeof(BoardDbContext).Assembly);

            builder.AddInterceptors(interceptors);
        });
        
        return services;
    }
}