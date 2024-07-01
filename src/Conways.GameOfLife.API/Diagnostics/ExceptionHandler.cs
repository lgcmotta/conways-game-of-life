using System.Net;
using Conways.GameOfLife.API.Exceptions;
using Conways.GameOfLife.API.Models;
using Conways.GameOfLife.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace Conways.GameOfLife.API.Diagnostics;

public class ExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken = default)
    {
        var response = exception switch
        {
            ArgumentNullException or ArgumentException or ArgumentOutOfRangeException => new 
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new ErrorResponse(["Bad input when processing request"])
            },
            ValidationFailedException validationException => new 
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new ErrorResponse(validationException.Errors.Select(error => $"'{error.Property}' {error.ErrorMessage}").ToArray())
            },
            BoardNotFoundException notFoundException => new
            {
                StatusCode = HttpStatusCode.NotFound,
                Content = new ErrorResponse([notFoundException.Message])
            },
            UnstableBoardException unstableBoardException => new
            {
                StatusCode = HttpStatusCode.UnprocessableEntity,
                Content = new ErrorResponse([unstableBoardException.Message])
            },
            _ => new 
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Content = new ErrorResponse(["Application failed to process request, please contact the support"])
            }
        };

        httpContext.Response.StatusCode = (int)response.StatusCode;
        
        await httpContext.Response.WriteAsJsonAsync(response.Content, cancellationToken)
            .ConfigureAwait(continueOnCapturedContext: false);
        
        return true;
    }
}