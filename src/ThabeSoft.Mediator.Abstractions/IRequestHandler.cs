namespace ThabeSoft.Mediator;

/// <summary>
/// 请求-响应处理器
/// </summary>
public interface IRequestHandler<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    ValueTask<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken = default);
}

/// <summary>
/// 请求处理器
/// </summary>
public interface IRequestHandler<in TRequest>
{
    ValueTask HandleAsync(TRequest request, CancellationToken cancellationToken = default);
}