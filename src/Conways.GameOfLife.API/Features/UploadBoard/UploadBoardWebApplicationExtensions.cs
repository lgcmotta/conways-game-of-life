namespace Conways.GameOfLife.API.Features.UploadBoard;

public static class UploadBoardWebApplicationExtensions
{
    public static WebApplication MapUploadBoardEndpoint(this WebApplication app)
    {
        app.MapPost("/api/boards", UploadBoardEndpoint.PostAsync)
            .WithName("UploadNewBoard")
            .WithDisplayName("Upload New Board")
            .WithOpenApi();
        
        return app;
    }
}