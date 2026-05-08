namespace ThabeSoft.Mediator;

/// <summary>
/// 中间件
/// </summary>
/// <typeparam name="TRequest"></typeparam>
public interface IMiddleware<TRequest, TResponse>
{
    ValueTask<TResponse> InvokeAsync(TRequest message, NextMiddleware<TRequest, TResponse> next, CancellationToken cancellationToken = default);
}


public interface IMiddleware<TRequest>
{
    ValueTask InvokeAsync(TRequest message, NextMiddleware<TRequest> next, CancellationToken cancellationToken = default);
}