using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.OpenTelemetry;

namespace Conways.GameOfLife.Infrastructure.Extensions;

[ExcludeFromCodeCoverage]
public static class LoggingBuilderExtensions
{
    public static ILoggingBuilder AddSerilogLogging(this ILoggingBuilder builder, IConfiguration configuration)
    {
        var serviceName = configuration.GetValue<string>("SERVICE_NAME");
        var serviceVersion = configuration.GetValue<string>("SERVICE_VERSION");
        var exporterEndpoint = configuration.GetValue<string>("EXPORTER_ENDPOINT");
        
        ArgumentException.ThrowIfNullOrWhiteSpace(serviceName);
        ArgumentException.ThrowIfNullOrWhiteSpace(serviceVersion);
        ArgumentException.ThrowIfNullOrWhiteSpace(exporterEndpoint);
        
        builder.ClearProviders();

        var logger = new LoggerConfiguration()
            .Enrich.WithProperty("service_name", serviceName)
            .Enrich.WithProperty("service_version", serviceVersion)
            .WriteTo.OpenTelemetry(options =>
            {
                options.Endpoint = exporterEndpoint;
                options.Protocol = OtlpProtocol.Grpc;
                options.ResourceAttributes = new Dictionary<string, object>
                {
                    ["service.name"] = serviceName,
                    ["service.version"] = serviceVersion
                };
            })
            .WriteTo.Conditional(_ => Debugger.IsAttached, sink => sink.Console())
            .CreateLogger();

        Log.Logger = logger;
        
        builder.AddSerilog(logger);

        return builder;
    }
}