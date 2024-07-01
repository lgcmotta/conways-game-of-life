using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Conways.GameOfLife.API.Features.NextGeneration;

public static class NextGenerationEndpoint
{
    public static async Task<IResult> GetAsync(
        [FromRoute] string boardId,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken = default)
    {
        var response = await mediator.Send(new NextGenerationQuery(boardId), cancellationToken)
            .ConfigureAwait(continueOnCapturedContext: false);

        return Results.Ok(response);
    }
}