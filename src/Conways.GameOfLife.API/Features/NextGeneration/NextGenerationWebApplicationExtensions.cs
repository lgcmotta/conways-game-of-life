namespace Conways.GameOfLife.API.Features.NextGeneration;

public static class NextGenerationWebApplicationExtensions
{
    public static WebApplication MapNextGenerationEndpoint(this WebApplication app)
    {
        app.MapGet("/api/boards/{boardId}/generations/next", NextGenerationQueryEndpoint.GetAsync)
            .WithName("GetNextGeneration")
            .WithDisplayName("Get Board Next Generation")
            .WithOpenApi();
        
        return app;
    }
}