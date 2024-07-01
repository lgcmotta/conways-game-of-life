using System.Net;
using Asp.Versioning;
using Conways.GameOfLife.API.Models;

namespace Conways.GameOfLife.API.Features.NextGenerations;

public static class NextGenerationsWebApplicationExtensions
{
    public static RouteGroupBuilder MapNextGenerationsEndpoint(this RouteGroupBuilder group, ApiVersion version)
    {
        const string contentType = "application/json";
        
        group.MapGet("/boards/{boardId}/generations/{generations:int}", NextGenerationsEndpoint.GetAsync)
            .WithName("GetNextGenerations")
            .WithDisplayName("Get Board Next x Generations")
            .WithOpenApi()
            .WithTags("Get Board's Next x Generation")
            .MapToApiVersion(version)
            .Produces<NextGenerationsResponse>(contentType: contentType)
            .Produces<ErrorResponse>((int)HttpStatusCode.BadRequest, contentType)
            .Produces<ErrorResponse>((int)HttpStatusCode.NotFound, contentType)
            .Produces<ErrorResponse>((int)HttpStatusCode.InternalServerError, contentType);
        
        return group;
    }
}