namespace ThabeSoft.Mediator;


/// <summary>
/// 请求管道
/// </summary>
public interface IRequestPipeline<TRequest>
    where TRequest : IRequest
{
    ValueTask InvokeAsync(TRequest request, CancellationToken cancellation = default);
}

/// <summary>
/// 请求管道
/// </summary>
public interface IRequestPipeline<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    ValueTask<TResponse> InvokeAsync(TRequest request, CancellationToken cancellation = default);
}
