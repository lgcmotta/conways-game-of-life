using System.Diagnostics.CodeAnalysis;
using Conways.GameOfLife.Infrastructure.Factories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Conways.GameOfLife.Infrastructure.PostgresSQL;

[ExcludeFromCodeCoverage]
public class BoardDesignTimeDbContextFactory : IDesignTimeDbContextFactory<BoardDbContext>
{
    public BoardDbContext CreateDbContext(string[] args)
    {
        var configuration = ConfigurationFactory.CreateConfiguration();

        var connectionString = configuration.GetConnectionString(nameof(BoardDbContext));

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException($"Connection string with key ${nameof(BoardDbContext)} was not found");
        }

        var options = new DbContextOptionsBuilder<BoardDbContext>()
            .UseNpgsql(connectionString, builder => builder.EnableRetryOnFailure(3))
            .Options;

        return new BoardDbContext(options);
    }
}