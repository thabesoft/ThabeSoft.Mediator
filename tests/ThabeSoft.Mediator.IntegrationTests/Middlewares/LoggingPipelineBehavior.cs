namespace ThabeSoft.Mediator.IntegrationTests.Middlewares;

public sealed class LoggingPipelineBehavior<TRequest, TResponse> : IRequestPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public ValueTask<TResponse> InvokeAsync(TRequest message, HandlerDelegate< TResponse> next, CancellationToken cancellationToken = default)
    {
        return next(cancellationToken);
    }
}