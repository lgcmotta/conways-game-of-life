using Conways.GameOfLife.API.Diagnostics;
using Conways.GameOfLife.API.Extensions;
using Conways.GameOfLife.API.Features.FinalGeneration;
using Conways.GameOfLife.API.Features.NextGeneration;
using Conways.GameOfLife.API.Features.NextGenerations;
using Conways.GameOfLife.API.Features.UploadBoard;
using Conways.GameOfLife.API.Middlewares;
using Conways.GameOfLife.Infrastructure.Extensions;
using Conways.GameOfLife.Infrastructure.PostgreSQL.Extensions;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddEnvironmentVariables();

builder.Logging.AddSerilogLogging(builder.Configuration);

builder.Services
    .AddBoardDbContexts(builder.Configuration)
    .AddHashIds(builder.Configuration)
    .AddCQRS()
    .AddFluentValidators()
    .AddExceptionHandler<ExceptionHandler>()
    .AddTransient<ExceptionMiddleware>()
    .AddEndpointsApiExplorer()
    .AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo { Title = "Conway's Game Of Life API", Version = "v1"});
    })
    .AddOpenTelemetryObservability(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>();

app.MapUploadBoardEndpoint()
    .MapNextGenerationEndpoint()
    .MapNextGenerationsEndpoint()
    .MapFinalGenerationEndpoint();

app.MapSwagger();

await app.Services
    .MigrateDatabaseAsync()
    .ConfigureAwait(continueOnCapturedContext: false);
    
await app.RunAsync()
    .ConfigureAwait(continueOnCapturedContext: false);

// ReSharper disable once ClassNeverInstantiated.Global
namespace Conways.GameOfLife.API
{
    public partial class Program
    {
        protected Program()
        { }
    }
}