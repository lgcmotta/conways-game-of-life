using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Formatting.Compact;
using Serilog.Sinks.OpenTelemetry;

namespace Conways.GameOfLife.Infrastructure.Extensions;

[ExcludeFromCodeCoverage]
public static class OpenTelemetryExtensions
{
    public static ILoggingBuilder AddSerilogLogging(this ILoggingBuilder builder, IConfiguration configuration)
    {
        var serviceName = configuration.GetValue<string>("SERVICE_NAME");
        var serviceVersion = configuration.GetValue<string>("SERVICE_VERSION");
        var exporterEndpoint = configuration.GetValue<string>("SEQ_ENDPOINT");
        var seqApiKey = configuration.GetValue<string>("SEQ_API_KEY");
        
        ArgumentException.ThrowIfNullOrWhiteSpace(serviceName);
        ArgumentException.ThrowIfNullOrWhiteSpace(serviceVersion);
        ArgumentException.ThrowIfNullOrWhiteSpace(exporterEndpoint);
        ArgumentException.ThrowIfNullOrWhiteSpace(seqApiKey);
        
        builder.ClearProviders();

        var logger = new LoggerConfiguration()
            .WriteTo.OpenTelemetry(options =>
            {
                options.Endpoint = exporterEndpoint;
                options.Protocol = OtlpProtocol.HttpProtobuf;
                options.Headers = new Dictionary<string, string>
                {
                    ["X-Seq-ApiKey"] = seqApiKey
                };
                options.ResourceAttributes = new Dictionary<string, object>
                {
                    ["service.name"] = serviceName,
                    ["service.version"] = serviceVersion
                };
            })
            .WriteTo.Conditional(_ => Debugger.IsAttached, sink => sink.Console(new CompactJsonFormatter()))
            .CreateLogger();

        Log.Logger = logger;
        
        builder.AddSerilog(logger);

        return builder;
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
            })
            .WithMetrics(builder =>
            {
                builder.AddAspNetCoreInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddEventCountersInstrumentation(options => options
                        .AddEventSources(
                            "Microsoft.AspNetCore.Hosting",
                            "Microsoft.AspNetCore.Http.Connections",
                            "Microsoft-AspNetCore-Server-Kestrel",
                            "System.Net.Http",
                            "System.Net.NameResolution",
                            "System.Net.Security"
                        )
                    )
                    .AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri(exporterEndpoint);
                        options.Protocol = OtlpExportProtocol.Grpc;
                    });
            });
        
        return services;
    }
}