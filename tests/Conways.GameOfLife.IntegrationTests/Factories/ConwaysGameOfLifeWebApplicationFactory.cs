using Conways.GameOfLife.Infrastructure.PostgresSQL.Interceptors;

namespace Conways.GameOfLife.IntegrationTests.Factories;

// ReSharper disable once ClassNeverInstantiated.Global
public class ConwaysGameOfLifeWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithPassword("p4ssw0rd")
        .WithPortBinding(5432, true)
        .WithDatabase("ConwaysGameOfLife")
        .Build();
    
    public async Task InitializeAsync()
    {
        await InitializePostgresContainerAsync();
    
        using var scope = Services.CreateScope();
        
        var context = scope.ServiceProvider.GetRequiredService<BoardDbContext>();
        
        await context.Database.MigrateAsync();
    }
    
    public new async Task DisposeAsync()
    {
        await base.DisposeAsync();
    }

    private async Task InitializePostgresContainerAsync(CancellationToken cancellationToken = default)
    {
        await _container.StartAsync(cancellationToken);
    }
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            RemoveDbContextOptions(services);

            var connectionString = _container.GetConnectionString();

            services.AddDbContext<BoardDbContext>((provider, optionsBuilder) =>
            {
                optionsBuilder.UseNpgsql(connectionString, pgsql =>
                {
                    pgsql.EnableRetryOnFailure(3);
                });

                var interceptors = InterceptorsAssemblyScanner.Scan(provider, typeof(BoardDbContext).Assembly);

                optionsBuilder.AddInterceptors(interceptors);
            });
        });
    }

    private static void RemoveDbContextOptions(IServiceCollection services)
    {
        var serviceDescriptor = services.FirstOrDefault(
            descriptor => descriptor.ServiceType == typeof(DbContextOptions<BoardDbContext>)
        );

        if (serviceDescriptor is not null)
        {
            services.Remove(serviceDescriptor);
        }
    }
}