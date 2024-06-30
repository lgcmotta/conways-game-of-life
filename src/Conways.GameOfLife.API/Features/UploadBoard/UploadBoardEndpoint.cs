using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Conways.GameOfLife.API.Features.UploadBoard;

public static class UploadBoardEndpoint
{
    public static async Task<IResult> PostAsync(
        [FromBody] UploadBoardCommand command,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken = default)
    {
        var response = await mediator.Send(command, cancellationToken)
            .ConfigureAwait(continueOnCapturedContext: false);
        
        return Results.Ok(response);
    }
}