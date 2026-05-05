namespace ThabeSoft.Mediator;


/// <summary>
/// 事件处理器
/// </summary>
/// <typeparam name="TEvent"></typeparam>
public interface IEventHandler<TEvent>
    where TEvent : IEvent
{
    ValueTask HandleAsync(TEvent @event, CancellationToken cancellationToken = default);
}