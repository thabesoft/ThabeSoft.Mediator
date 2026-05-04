namespace ThabeSoft.Mediator.Warppers;


/// <summary>
/// 命令包装器
/// </summary>
public interface ICommandHandlerWarpper : IWarpper
{
    Task HandleAsync(ICommand command, CancellationToken cancellationToken = default);
}
