namespace ThabeSoft.Mediator;


/// <summary>
/// 命令分发器
/// </summary>
public interface ICommandDispatcher
{
    Task DispatchAsync(ICommand command, CancellationToken cancellationToken = default);
    Task<TResult> DispatchAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default);
}

public interface ICommandWarpper
{
    Task HandleAsync(ICommand command, CancellationToken cancellationToken = default);
    Task<TResult> HandleAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default);
}