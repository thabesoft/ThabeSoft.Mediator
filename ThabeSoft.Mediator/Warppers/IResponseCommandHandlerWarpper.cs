namespace ThabeSoft.Mediator.Warppers;

/// <summary>
/// 响应命令包装器
/// </summary>
/// <typeparam name="TResult"></typeparam>
public interface IResponseCommandHandlerWarpper : IWarpper
{
    Task<TResult> HandleAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default);
}