namespace ThabeSoft.Mediator;


/// <summary>
/// 通知处理器
/// </summary>
public interface INotificationHandler<TNotification>
    where TNotification : INotification
{
    ValueTask HandleAsync(TNotification notification, CancellationToken cancellationToken = default);
}