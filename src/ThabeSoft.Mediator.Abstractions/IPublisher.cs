namespace ThabeSoft.Mediator;

/// <summary>
/// 发布器
/// </summary>
public interface IPublisher
{
    ValueTask PublishAsync<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification;
}