namespace Conways.GameOfLife.IntegrationTests.Factories;

// ReSharper disable once ClassNeverInstantiated.Global
public class ConwaysGameOfLifeWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithPassword("p4ssw0rd")
        .WithPortBinding(5432, true)
        .WithDatabase("ConwaysGameOfLife")
        .Build();

    public async Task InitializeMongoDbContainerAsync(CancellationToken cancellationToken = default)
    {
        await _container.StartAsync(cancellationToken);
    }
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            RemoveDbContextOptions(services);

            var connectionString = _container.GetConnectionString();

            services.AddNpgsql<BoardDbContext>(connectionString);
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