namespace ThabeSoft.Mediator;

/// <summary>
/// 通知管道行为
/// </summary>
public interface INotificationPipelineBehavior<TNotification>
    where TNotification : INotification
{
    ValueTask InvokeAsync(
        TNotification request,
        HandlerDelegate next,
        CancellationToken cancellationToken = default);
}