using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Conways.GameOfLife.API.Behaviors;
using FluentValidation;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Conways.GameOfLife.API.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCQRS(this IServiceCollection services)
    {
        return services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssemblyContaining<Program>();
            config.AddOpenBehavior(typeof(LoggingBehavior<,>));
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
            config.Lifetime = ServiceLifetime.Scoped;
        });
    }

    public static IServiceCollection AddFluentValidators(this IServiceCollection services)
    {
        return services.AddValidatorsFromAssemblyContaining<Program>();
    }

    public static IServiceCollection AddOpenTelemetryObservability(this IServiceCollection services, IConfiguration configuration)
    {
        var serviceName = configuration.GetValue<string>("SERVICE_NAME");
        var serviceNamespace = configuration.GetValue<string>("SERVICE_NAMESPACE");
        var serviceVersion = configuration.GetValue<string>("SERVICE_VERSION");
        var autoGenerateServiceInstanceId = configuration.GetValue<bool>("AUTOGENERATE_SERVICE_INSTANCE_ID");
        var exporterEndpoint = configuration.GetValue<string>("EXPORTER_ENDPOINT");
        
        ArgumentException.ThrowIfNullOrWhiteSpace(serviceName);
        ArgumentException.ThrowIfNullOrWhiteSpace(serviceNamespace);
        ArgumentException.ThrowIfNullOrWhiteSpace(serviceVersion);
        ArgumentException.ThrowIfNullOrWhiteSpace(exporterEndpoint);

        services.AddOpenTelemetry()
            .ConfigureResource(builder =>
            {
                builder.AddService(serviceName, serviceNamespace, serviceVersion, autoGenerateServiceInstanceId)
                    .AddTelemetrySdk()
                    .AddEnvironmentVariableDetector();
            })
            .WithTracing(builder =>
            {
                builder.SetSampler<AlwaysOnSampler>()
                    .AddAspNetCoreInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation(options => options.SetDbStatementForText = true)
                    .AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri(exporterEndpoint);
                        options.Protocol = OtlpExportProtocol.Grpc;
                    });

                if (Debugger.IsAttached)
                {
                    builder.AddConsoleExporter();
                }
            });
        
        return services;
    }
}