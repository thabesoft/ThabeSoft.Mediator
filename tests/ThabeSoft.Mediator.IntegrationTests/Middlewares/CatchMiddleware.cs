namespace ThabeSoft.Mediator.IntegrationTests.Middlewares;



sealed class CatchMiddleware<TRequest, TResponse> : IMiddleware<TRequest, TResponse>
{
    public async ValueTask<TResponse> InvokeAsync(TRequest message, NextMiddleware<TRequest, TResponse> next, CancellationToken cancellationToken = default)
    {
        Console.Write("异常捕获开始{ ");
        TResponse result;

        try
        {
            result = await next(message, cancellationToken);
        }
        catch (Exception ex)
        {
            Console.Write($"捕获到 {ex.GetType().Name}");
            result = default!;
        }

        Console.Write(" }异常捕获结束");
        return result;
    }
}