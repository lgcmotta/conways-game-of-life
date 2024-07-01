namespace Conways.GameOfLife.API.Features.FinalGeneration;

public static class FinalGenerationWebApplicationExtensions
{
    public static WebApplication MapFinalGenerationEndpoint(this WebApplication app)
    {
        app.MapGet("/api/boards/{boardId}/generations/final", FinalGenerationEndpoint.GetAsync)
            .WithName("GetFinalGeneration")
            .WithDisplayName("Get Board Final Generation After x Attempts")
            .WithOpenApi();
        
        return app;
    }
}