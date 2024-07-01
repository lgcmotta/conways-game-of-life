using Conways.GameOfLife.API.Diagnostics;
using Conways.GameOfLife.API.Extensions;
using Conways.GameOfLife.API.Features.NextGeneration;
using Conways.GameOfLife.API.Features.NextGenerations;
using Conways.GameOfLife.API.Features.UploadBoard;
using Conways.GameOfLife.API.Middlewares;
using Conways.GameOfLife.Infrastructure.Extensions;
using Conways.GameOfLife.Infrastructure.PostgresSQL.Extensions;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddBoardDbContext(builder.Configuration)
    .AddHashIds(builder.Configuration)
    .AddCQRS()
    .AddFluentValidators()
    .AddExceptionHandler<ExceptionHandler>()
    .AddTransient<ExceptionMiddleware>()
    .AddEndpointsApiExplorer()
    .AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo { Title = "Conway's Game Of Life API", Version = "v1"});
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>();

app.MapUploadBoardEndpoint()
    .MapNextGenerationEndpoint()
    .MapNextGenerationsEndpoint();

app.MapSwagger();

await app.Services
    .MigrateDatabaseAsync()
    .ConfigureAwait(continueOnCapturedContext: false);
    
await app.RunAsync()
    .ConfigureAwait(continueOnCapturedContext: false);

// ReSharper disable once ClassNeverInstantiated.Global
public partial class Program
{
    protected Program()
    { }
}