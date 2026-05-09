namespace ThabeSoft.Mediator.UnitTests.Datas;

public sealed class NotificationHandler : INotificationHandler<Notification>
{
    public ValueTask HandleAsync(Notification notification, CancellationToken cancellationToken = default) => default;
}
