using Microsoft.Extensions.DependencyInjection;
using System.Collections;

namespace ThabeSoft.Mediator;


/// <summary>
/// 默认实现
/// </summary>
public sealed class Mediator(IServiceProvider services) : IMediator
{
    public ValueTask SendAsync<TRequest>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequest
    {
        if (request is null) throw new ArgumentNullException(nameof(request), "请求不可为空");

        var handler = services.GetRequiredService<IRequestHandler<TRequest>>();
        var behaviors = TryGetServices<IRequestPipelineBehavior<TRequest>>();

        // 没有行为
        if (behaviors.Length == 0) return handler.HandleAsync(request, cancellationToken);

        var pipe_line = new RequestPipeline<TRequest>(request, handler, behaviors);
        return pipe_line.InvokeAsync(cancellationToken);
    }

    public ValueTask<TResponse> SendAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : IRequest<TResponse>
    {
        if (request is null) throw new ArgumentNullException(nameof(request), "请求不可为空");

        var handler = services.GetRequiredService<IRequestHandler<TRequest, TResponse>>();
        var behaviors = TryGetServices<IRequestPipelineBehavior<TRequest, TResponse>>();

        // 没有行为
        if (behaviors.Length == 0) return handler.HandleAsync(request, cancellationToken);

        var pipe_line = new RequestPipeline<TRequest, TResponse>(request, handler, behaviors);
        return pipe_line.InvokeAsync(cancellationToken);
    }

    public ValueTask PublishAsync<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification
    {
        if (notification is null) throw new ArgumentNullException(nameof(notification), "通知不可为空");

        var handlers = services.GetServices<INotificationHandler<TNotification>>().ToArray();
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
                var pipe_line = new NotifyPipeline<TNotification>(notification, handler, pipeline_behaviors);
                return pipe_line.InvokeAsync(cancellationToken).AsTask();
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