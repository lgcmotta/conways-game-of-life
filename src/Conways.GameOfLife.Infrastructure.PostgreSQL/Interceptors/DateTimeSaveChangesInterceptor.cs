using Conways.GameOfLife.Domain.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Conways.GameOfLife.Infrastructure.PostgreSQL.Interceptors;

public class DateTimeSaveChangesInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = new CancellationToken())
    {
        ArgumentNullException.ThrowIfNull(eventData);

        var entityEntries = eventData.Context?.ChangeTracker.Entries<IEntity>() ?? [];

        foreach (var entityEntry in entityEntries)
        {
            if (entityEntry.State == EntityState.Added)
            {
                entityEntry.Property<DateTime>("CreatedAt").CurrentValue = DateTime.UtcNow;
                
                if (entityEntry.Properties.Any(propertyEntry => propertyEntry.Metadata.IsShadowProperty() && propertyEntry.Metadata.Name == "UpdatedAt"))
                {
                    entityEntry.Property<DateTime?>("UpdatedAt").CurrentValue = null;
                }
            }

            if (entityEntry.State == EntityState.Modified)
            {
                entityEntry.Property<DateTime?>("UpdatedAt").CurrentValue = DateTime.UtcNow;
            }
        }
        
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}