using System.Net;
using Asp.Versioning;
using Conways.GameOfLife.API.Models;

namespace Conways.GameOfLife.API.Features.FinalGeneration;

public static class FinalGenerationWebApplicationExtensions
{
    public static RouteGroupBuilder MapFinalGenerationEndpoint(this RouteGroupBuilder group, ApiVersion version)
    {
        const string contentType = "application/json";
        
        group.MapGet("/boards/{boardId}/generations/final", FinalGenerationEndpoint.GetAsync)
            .WithName("GetFinalGeneration")
            .WithDisplayName("Get Board Final Generation After x Attempts")
            .WithOpenApi()
            .WithTags("Get Board's Final Generation")
            .MapToApiVersion(version)
            .Produces<FinalGenerationResponse>(contentType: contentType)
            .Produces<ErrorResponse>((int)HttpStatusCode.BadRequest, contentType)
            .Produces<ErrorResponse>((int)HttpStatusCode.NotFound, contentType)
            .Produces<ErrorResponse>((int)HttpStatusCode.UnprocessableEntity, contentType)
            .Produces<ErrorResponse>((int)HttpStatusCode.InternalServerError, contentType);
        
        return group;
    }
}