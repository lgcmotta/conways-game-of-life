using Microsoft.Extensions.Configuration;

namespace Conways.GameOfLife.Infrastructure.Factories;

public static class ConfigurationFactory
{
    public static IConfiguration CreateConfiguration()
    {
        var configurationBuilder = new ConfigurationBuilder();

        configurationBuilder.AddJsonFile("appsettings.json");

        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        if (!string.IsNullOrWhiteSpace(environment))
        {
            configurationBuilder.AddJsonFile(
                path: $"appsettings.{environment}.json",
                optional: true,
                reloadOnChange: true
            );
        }

        configurationBuilder.AddEnvironmentVariables();

        return configurationBuilder.Build();
    }
}