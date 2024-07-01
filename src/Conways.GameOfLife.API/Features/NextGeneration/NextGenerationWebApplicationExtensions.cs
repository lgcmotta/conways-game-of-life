using System.Net;
using Asp.Versioning;
using Conways.GameOfLife.API.Models;

namespace Conways.GameOfLife.API.Features.NextGeneration;

public static class NextGenerationWebApplicationExtensions
{
    public static RouteGroupBuilder MapNextGenerationEndpoint(this RouteGroupBuilder group, ApiVersion version)
    {
        const string contentType = "application/json";
        
        group.MapGet(pattern: "/boards/{boardId}/generations/next", handler: NextGenerationEndpoint.GetAsync)
            .WithName(endpointName: "GetNextGeneration")
            .WithDisplayName(displayName: "Get Board Next Generation")
            .WithOpenApi()
            .WithTags(tags: "Get Board's Next Generation")
            .MapToApiVersion(apiVersion: version)
            .Produces<NextGenerationResponse>(contentType: contentType)
            .Produces<ErrorResponse>((int)HttpStatusCode.BadRequest, contentType)
            .Produces<ErrorResponse>((int)HttpStatusCode.NotFound, contentType)
            .Produces<ErrorResponse>((int)HttpStatusCode.InternalServerError, contentType);

        return group;
    }
}