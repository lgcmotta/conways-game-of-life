using Microsoft.EntityFrameworkCore;

namespace Conways.GameOfLife.Infrastructure.PostgreSQL;

public sealed class BoardDbContext : DbContext
{
    public BoardDbContext(DbContextOptions<BoardDbContext> options) : base(options)
    {
        ChangeTracker.AutoDetectChangesEnabled = true;
        ChangeTracker.LazyLoadingEnabled = false;
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BoardDbContext).Assembly);
    }
}