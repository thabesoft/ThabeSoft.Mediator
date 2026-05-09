namespace ThabeSoft.Mediator.UnitTests.Datas;

public sealed class NotificationPipelineBehavior : INotificationPipelineBehavior<Notification>
{
    public ValueTask InvokeAsync(Notification request, HandlerDelegate next, CancellationToken cancellationToken = default)
    {
        return next(cancellationToken);
    }
}