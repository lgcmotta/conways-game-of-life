namespace Conways.GameOfLife.API.Features.CreateBoard;

public static class CreateBoardWebApplicationExtensions
{
    public static WebApplication MapUploadBoardEndpoint(this WebApplication app)
    {
        app.MapPost("/api/boards", CreateBoardEndpoint.PostAsync)
            .WithName("CreateNewBoard")
            .WithDisplayName("Create New Board")
            .WithOpenApi()
            .WithTags("Create a New Board");
        
        return app;
    }
}