namespace ThabeSoft.Mediator;

/// <summary>
/// 通知管道
/// </summary>
internal struct NotifyPipeline<TNotification>(
        TNotification notification,
        INotificationHandler<TNotification> handler,
        INotificationPipelineBehavior<TNotification>[] behaviors
    ) where TNotification : INotification
{
    // 当前执行行为索引
    private int _index;

    public ValueTask InvokeAsync(CancellationToken cancellationToken)
    {
        return MoveNext(cancellationToken);
    }

    private ValueTask MoveNext(CancellationToken cancellationToken)
    {
        if (_index >= behaviors.Length) return handler.HandleAsync(notification, cancellationToken);

        var behavior = behaviors[_index++];
        return behavior.InvokeAsync(notification, MoveNext, cancellationToken);
    }
}