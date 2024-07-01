using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Conways.GameOfLife.API.Features.FinalGeneration;

public static class FinalGenerationEndpoint
{
    public static async Task<IResult> GetAsync(
        string boardId,
        [FromQuery] int? maxAttempts,
        [FromServices] IMediator mediator,
        [FromServices] IConfiguration configuration,
        CancellationToken cancellationToken = default)
    {
        maxAttempts ??= configuration.GetValue<int>("Board:MaxAttempts");
        
        var response = await mediator.Send(new FinalGenerationQuery(boardId, maxAttempts.Value), cancellationToken)
            .ConfigureAwait(continueOnCapturedContext: false);
        
        return Results.Ok(response);
    }
}