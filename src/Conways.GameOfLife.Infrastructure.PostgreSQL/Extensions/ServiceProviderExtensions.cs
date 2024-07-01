using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Conways.GameOfLife.Infrastructure.PostgreSQL.Extensions;

public static class ServiceProviderExtensions
{
    public static async Task MigrateDatabaseAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<BoardDbContext>();

        await context.Database.MigrateAsync().ConfigureAwait(continueOnCapturedContext: false);
    }
}