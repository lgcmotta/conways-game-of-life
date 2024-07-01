using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Conways.GameOfLife.API.Features.NextGenerations;

public static class NextGenerationsEndpoint
{
    public static async Task<IResult> GetAsync(
        string boardId,
        int generations,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken = default
    )
    {
        var response = await mediator.Send(new NextGenerationsQuery(boardId, generations), cancellationToken)
            .ConfigureAwait(continueOnCapturedContext: false);

        return Results.Ok(response);
    }
}