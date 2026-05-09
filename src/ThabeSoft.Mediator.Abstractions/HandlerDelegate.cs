namespace ThabeSoft.Mediator;


/// <summary>
/// 处理器委托
/// </summary>
/// <param name="cancellationToken"></param>
/// <returns></returns>
public delegate ValueTask HandlerDelegate(CancellationToken cancellationToken);

/// <summary>
/// 处理器委托
/// </summary>
/// <typeparam name="TResponse"></typeparam>
/// <param name="cancellationToken"></param>
/// <returns></returns>
public delegate ValueTask<TResponse> HandlerDelegate<TResponse>(CancellationToken cancellationToken);