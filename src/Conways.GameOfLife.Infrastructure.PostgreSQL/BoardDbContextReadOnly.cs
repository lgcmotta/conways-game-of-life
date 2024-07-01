using Microsoft.EntityFrameworkCore;

namespace Conways.GameOfLife.Infrastructure.PostgreSQL;

public sealed class BoardDbContextReadOnly : DbContext
{
    public BoardDbContextReadOnly(DbContextOptions<BoardDbContextReadOnly> options) : base(options)
    {
        ChangeTracker.AutoDetectChangesEnabled = false;
        ChangeTracker.LazyLoadingEnabled = false;
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BoardDbContextReadOnly).Assembly);
    }
}