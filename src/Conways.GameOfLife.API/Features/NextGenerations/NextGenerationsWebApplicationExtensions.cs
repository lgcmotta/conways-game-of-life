namespace Conways.GameOfLife.API.Features.NextGenerations;

public static class NextGenerationsWebApplicationExtensions
{
    public static WebApplication MapNextGenerationsEndpoint(this WebApplication app)
    {
        app.MapGet("/api/boards/{boardId}/generations/{generations:int}", NextGenerationsEndpoint.GetAsync)
            .WithName("GetNextGenerations")
            .WithDisplayName("Get Board Next x Generations")
            .WithOpenApi();
        
        return app;
    }
}