namespace ThabeSoft.Mediator.Benchmark.Handlers;

public abstract class NotificationHandlerBase<TNotification> :
    INotificationHandler<TNotification>,
    MediatR.INotificationHandler<TNotification>,
    DispatchR.Abstractions.Notification.INotificationHandler<TNotification>

    where TNotification :  INotification,
        DispatchR.Abstractions.Notification.INotification,
        MediatR.INotification

{
    protected abstract ValueTask HandleAsync(TNotification notification, CancellationToken cancellationToken);


    Task MediatR.INotificationHandler<TNotification>.Handle(TNotification notification, CancellationToken cancellationToken)
    {
        return HandleAsync(notification, cancellationToken).AsTask();
    }
    ValueTask DispatchR.Abstractions.Notification.INotificationHandler<TNotification>.Handle(TNotification notification, CancellationToken cancellationToken)
    {
        return HandleAsync(notification, cancellationToken);
    }
    ValueTask INotificationHandler<TNotification>.HandleAsync(TNotification notification, CancellationToken cancellationToken)
    {
        return HandleAsync(notification, cancellationToken);
    }
}