using Microsoft.Extensions.DependencyInjection;

namespace ThabeSoft.Mediator;


/// <summary>
/// 默认实现
/// </summary>
public sealed class Mediator(IServiceProvider services) : IMediator
{
    public ValueTask SendAsync<TRequest>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : IRequest
    {
        if (request is null) throw new ArgumentNullException(nameof(request), "请求不可为空");

        var pipeline = services.GetService<IRequestPipeline<TRequest>>();
        if (pipeline is not null) return pipeline.InvokeAsync(request, cancellationToken);

        var handler = services.GetRequiredService<IRequestHandler<TRequest>>();
        return handler.HandleAsync(request, cancellationToken);
    }
    public ValueTask<TResponse> SendAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : IRequest<TResponse>
    {
        if (request is null) throw new ArgumentNullException(nameof(request), "请求不可为空");

        // 获取管道
        var pipeline = services.GetService<IRequestPipeline<TRequest, TResponse>>();
        if (pipeline is not null) return pipeline.InvokeAsync(request, cancellationToken);

        var handler = services.GetRequiredService<IRequestHandler<TRequest, TResponse>>();
        return handler.HandleAsync(request, cancellationToken);
    }
    public ValueTask PublishAsync<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification
    {
        if (notification is null) throw new ArgumentNullException(nameof(notification), "通知不可为空");

        var handlers = TryGetServices<INotificationHandler<TNotification>>();
        var pipeline_behaviors = TryGetServices<INotificationPipelineBehavior<TNotification>>();

        // 没有处理器
        if (handlers.Length == 0) return default;

        // 没有行为
        if (pipeline_behaviors.Length == 0)
        {
            var tasks = handlers.Select(handler => handler.HandleAsync(notification, cancellationToken).AsTask());
            return new ValueTask(Task.WhenAll(tasks));
        }
        else
        {
            var tasks = handlers.Select(handler =>
            {
                var pipeline = services.GetRequiredService<INotificationPipeline<TNotification>>();
                if (pipeline is not null) return pipeline.InvokeAsync(notification, cancellationToken).AsTask();

                var handlera = services.GetRequiredService<INotificationHandler<TNotification>>();
                return handler.HandleAsync(notification, cancellationToken).AsTask();
            });

            return new ValueTask(Task.WhenAll(tasks));
        }
    }

    /// <summary>
    /// 微软的 GetServices{T} 是 GetRequiredService{IEnumerable{T}} 会引发异常, 这个如果不存在返回空集合
    /// </summary>
    private T[] TryGetServices<T>()
    {
        var items = services.GetService<IEnumerable<T>>();
        return items?.ToArray() ?? [];
    }
}