using Microsoft.AspNetCore.Diagnostics;

namespace Conways.GameOfLife.API.Middlewares;

public class ExceptionMiddleware : IMiddleware
{
    private readonly IExceptionHandler _exceptionHandler;

    public ExceptionMiddleware(IExceptionHandler exceptionHandler)
    {
        _exceptionHandler = exceptionHandler;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context).ConfigureAwait(continueOnCapturedContext: false);
        }
        catch (Exception exception)
        {
            await _exceptionHandler.TryHandleAsync(context, exception, context.RequestAborted)
                .ConfigureAwait(continueOnCapturedContext: false);
        }
    }
}