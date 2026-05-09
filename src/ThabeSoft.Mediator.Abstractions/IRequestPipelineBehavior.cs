namespace ThabeSoft.Mediator;

/// <summary>
/// 请求管道行为
/// </summary>
/// <typeparam name="TRequest">请求</typeparam>
public interface IRequestPipelineBehavior<TRequest>
    where TRequest : IRequest
{
    ValueTask InvokeAsync(
        TRequest request,
        HandlerDelegate next,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// 请求管道行为
/// </summary>
/// <typeparam name="TRequest">请求</typeparam>
/// <typeparam name="TResponse">响应</typeparam>
public interface IRequestPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    ValueTask<TResponse> InvokeAsync(
        TRequest request,
        HandlerDelegate<TResponse> next,
        CancellationToken cancellationToken = default);
}