using Conways.GameOfLife.Infrastructure.PostgresSQL.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddBoardDbContext(builder.Configuration);

var app = builder.Build();

await app.RunAsync();

public partial class Program
{
    protected Program()
    { }
}