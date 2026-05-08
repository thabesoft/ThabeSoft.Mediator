namespace ThabeSoft.Mediator;


/// <summary>
/// 中间件管道
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
/// <param name="request"></param>
/// <param name="cancellationToken"></param>
/// <returns></returns>
public delegate ValueTask<TResponse> Pipeline<TRequest, TResponse>(
        TRequest request,
        CancellationToken cancellationToken = default
    ) where TRequest : IRequest<TResponse>;


/// <summary>
/// 中间件管道
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <param name="request"></param>
/// <param name="cancellationToken"></param>
/// <returns></returns>
public delegate ValueTask Pipeline<TRequest>(
        TRequest request,
        CancellationToken cancellationToken = default
    ) where TRequest : IRequest;