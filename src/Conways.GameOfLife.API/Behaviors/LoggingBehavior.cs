using Conways.GameOfLife.Infrastructure.Extensions;
using MediatR;

namespace Conways.GameOfLife.API.Behaviors;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseRequest
    where TResponse : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest,TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }
    
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var behaviorName = typeof(LoggingBehavior<TRequest, TResponse>).GetGenericTypeName();

        var requestType = typeof(TRequest);

        try
        {
            _logger.LogInformation("[{Behavior}] - Handling request of type {RequestType}", behaviorName, requestType);

            var response = await next().ConfigureAwait(continueOnCapturedContext: false);

            _logger.LogInformation("[{Behavior}] - Request of type {RequestType} handled successfully", behaviorName, requestType);

            return response;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "[{Behavior}] - An exception occurred while handling request of type {RequestType}", behaviorName, requestType);

            throw;
        }
    }
}