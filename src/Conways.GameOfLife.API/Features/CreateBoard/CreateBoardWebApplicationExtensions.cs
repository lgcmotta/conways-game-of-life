using System.Net;
using Asp.Versioning;
using Conways.GameOfLife.API.Models;

namespace Conways.GameOfLife.API.Features.CreateBoard;

public static class CreateBoardWebApplicationExtensions
{
    public static RouteGroupBuilder MapCreateBoardEndpoint(this RouteGroupBuilder group, ApiVersion version)
    {
        const string contentType = "application/json";
        
        group.MapPost("/boards", CreateBoardEndpoint.PostAsync)
            .WithName("CreateNewBoard")
            .WithDisplayName("Create New Board")
            .WithOpenApi()
            .WithTags("Create a New Board")
            .MapToApiVersion(version)
            .Produces<CreateBoardResponse>(contentType: contentType)
            .Produces<ErrorResponse>((int)HttpStatusCode.BadRequest, contentType)
            .Produces<ErrorResponse>((int)HttpStatusCode.InternalServerError, contentType);
        
        return group;
    }
}