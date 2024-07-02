using Asp.Versioning;
using Conways.GameOfLife.API.Diagnostics;
using Conways.GameOfLife.API.Extensions;
using Conways.GameOfLife.API.Features.CreateBoard;
using Conways.GameOfLife.API.Features.FinalGeneration;
using Conways.GameOfLife.API.Features.NextGeneration;
using Conways.GameOfLife.API.Features.NextGenerations;
using Conways.GameOfLife.API.Middlewares;
using Conways.GameOfLife.Infrastructure.Extensions;
using Conways.GameOfLife.Infrastructure.PostgreSQL.Extensions;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddEnvironmentVariables();

builder.Logging.AddSerilogLogging(builder.Configuration);

var v1 = new ApiVersion(1, minorVersion: 0);

builder.Services
    .AddBoardDbContexts(builder.Configuration)
    .AddHashIds(builder.Configuration)
    .AddCQRS()
    .AddFluentValidators()
    .AddExceptionHandler<ExceptionHandler>()
    .AddTransient<ExceptionMiddleware>()
    .AddApiVersioning(options =>
    {
        options.ReportApiVersions = true;
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ApiVersionReader = new UrlSegmentApiVersionReader();
        options.DefaultApiVersion = v1;
    })
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'V";
        options.SubstituteApiVersionInUrl = true;
    })
    .EnableApiVersionBinding();

builder.Services.AddEndpointsApiExplorer()
    .AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1",
            new OpenApiInfo { Title = "Conway's Game Of Life API", Version = "v1" });
    })
    .AddOpenTelemetryObservability(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>();

var v1Set = app.NewApiVersionSet()
    .HasApiVersion(v1)
    .ReportApiVersions()
    .Build();

var api = app.MapGroup("/api/v{version:apiVersion}")
    .WithApiVersionSet(v1Set);

api.MapCreateBoardEndpoint(v1)
    .MapNextGenerationEndpoint(v1)
    .MapNextGenerationsEndpoint(v1)
    .MapFinalGenerationEndpoint(v1);

app.MapSwagger();

await app.Services
    .MigrateDatabaseAsync()
    .ConfigureAwait(continueOnCapturedContext: false);

await app.RunAsync()
    .ConfigureAwait(continueOnCapturedContext: false);

public partial class Program
{
    protected Program()
    {
    }
}