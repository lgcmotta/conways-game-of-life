using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Conways.GameOfLife.API.Features.CreateBoard;

public static class CreateBoardEndpoint
{
    public static async Task<IResult> PostAsync(
        [FromBody] CreateBoardCommand command,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken = default)
    {
        var response = await mediator.Send(command, cancellationToken)
            .ConfigureAwait(continueOnCapturedContext: false);
        
        return Results.Ok(response);
    }
}