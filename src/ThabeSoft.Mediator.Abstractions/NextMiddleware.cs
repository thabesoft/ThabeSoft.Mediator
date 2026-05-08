namespace ThabeSoft.Mediator;


/// <summary>
/// 下一个中间件委托
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
/// <param name="cancellationToken"></param>
/// <returns></returns>
public delegate ValueTask<TResponse> NextMiddleware<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken);


/// <summary>
/// 下一个中间件委托
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <param name="request"></param>
/// <param name="cancellationToken"></param>
/// <returns></returns>
public delegate ValueTask NextMiddleware<TRequest>(TRequest request, CancellationToken cancellationToken);