using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Conways.GameOfLife.Infrastructure.PostgresSQL.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBoardDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(nameof(BoardDbContext));

        var retries = configuration.GetValue<int>("PostgresSettings:RetryCount");
        
        services.AddNpgsql<BoardDbContext>(connectionString, builder => builder.EnableRetryOnFailure(retries));
        
        return services;
    }
}