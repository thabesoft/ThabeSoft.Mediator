namespace ThabeSoft.Mediator;

/// <summary>
/// 通知管道
/// </summary>
public interface INotificationPipeline<TNotification>
    where TNotification : INotification
{
    ValueTask InvokeAsync(TNotification request, CancellationToken cancellation = default);
}