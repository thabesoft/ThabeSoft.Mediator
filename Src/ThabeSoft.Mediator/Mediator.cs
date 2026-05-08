using ThabeSoft.Mediator.Extensions;

namespace ThabeSoft.Mediator;


/// <summary>
/// 默认实现
/// </summary>
/// <param name="services"></param>
public sealed class Mediator(IServiceProvider services) : IMediator
{
    public ValueTask SendAsync<TRequest>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequest
    {
        if (request is null) throw new ArgumentNullException(nameof(request), "请求不可为空");

        return services.GetRequiredRequestPipeline<TRequest>().Invoke(request, cancellationToken);
    }

    public ValueTask<TResponse> SendAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : IRequest<TResponse>
    {
        if (request is null) throw new ArgumentNullException(nameof(request), "请求不可为空");

        return services.GetRequiredRequestPipeline<TRequest, TResponse>().Invoke(request, cancellationToken);
    }

    public ValueTask PublishAsync<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification
    {
        if (notification is null) throw new ArgumentNullException(nameof(notification), "通知不可为空");

        var handlers = services.GetNotificationHandlers<TNotification>().ToArray();
        var handler_length = handlers.Length;

        if (handler_length <= 0) return default;

        Parallel.ForEach(handlers, x =>
        {
            x.HandleAsync(notification, cancellationToken);
        });

        return default;
    }
}