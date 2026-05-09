namespace ThabeSoft.Mediator.IntegrationTests.Middlewares;

public sealed class LoggingMiddleware<TRequest, TResponse> : IMiddleware<TRequest, TResponse>
{
    public async ValueTask<TResponse> InvokeAsync(TRequest message, RequestHandlerDelegateObsolete<TRequest, TResponse> next, CancellationToken cancellationToken = default)
    {
        Console.Write("日志开始{ ");
        var result = await next(message, cancellationToken);
        Console.Write(" }日志结束");

        return result;
    }
}