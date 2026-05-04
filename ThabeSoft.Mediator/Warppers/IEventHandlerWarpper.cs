namespace ThabeSoft.Mediator.Warppers;

/// <summary>
/// 事件处理器包装器
/// </summary>
public interface IEventHandlerWarpper : IWarpper
{
    Task HandleAsync(IEvent @event, CancellationToken cancellationToken = default);
}