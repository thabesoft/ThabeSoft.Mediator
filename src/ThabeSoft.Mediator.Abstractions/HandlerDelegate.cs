namespace ThabeSoft.Mediator;


/// <summary>
/// 处理器委托
/// </summary>
public delegate ValueTask HandlerDelegate(CancellationToken cancellationToken);

/// <summary>
/// 处理器委托
/// </summary>
public delegate ValueTask<TResponse> HandlerDelegate<TResponse>(CancellationToken cancellationToken);