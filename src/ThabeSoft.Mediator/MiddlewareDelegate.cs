namespace ThabeSoft.Mediator;


/// <summary>
/// 中间件委托
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
/// <param name="core"></param>
/// <param name="cancellationToken"></param>
/// <returns></returns>

public delegate ValueTask<TResponse> MiddlewareDelegate<TRequest, TResponse>(
        TRequest request,
        NextMiddleware<TRequest, TResponse> core,
        CancellationToken cancellationToken
    ) where TRequest : IRequest<TResponse>;

/// <summary>
/// 中间件委托
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <param name="request"></param>
/// <param name="core"></param>
/// <param name="cancellationToken"></param>
/// <returns></returns>
public delegate ValueTask MiddlewareDelegate<TRequest>(
        TRequest request,
        NextMiddleware<TRequest> core,
        CancellationToken cancellationToken
    ) where TRequest : IRequest;