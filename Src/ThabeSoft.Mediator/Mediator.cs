using System.Runtime.CompilerServices;

namespace ThabeSoft.Mediator;


/// <summary>
/// 默认实现
/// </summary>
/// <param name="services"></param>
public sealed class Mediator(IServiceProvider services) : IMediator
{
    public ValueTask SendAsync<TRequest>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : IRequest
    {
        if (request is null) throw new ArgumentNullException(nameof(request), "请求不可为空");
        return RequestHandlerSlot<TRequest>.Handler.Invoke(services, request, cancellationToken);
    }

    public ValueTask<TResponse> SendAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : IRequest<TResponse>
    {
        if (request is null) throw new ArgumentNullException(nameof(request), "请求不可为空");
        return RequestHandlerSlot<TRequest, TResponse>.Handler.Invoke(services, request, cancellationToken);
    }

    public ValueTask PublishAsync<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification
    {
        if (notification is null) throw new ArgumentNullException(nameof(notification), "通知不可为空");

        var handlers = NotificationHandlerSlot<TNotification>.GetHandlers(services);
        var handler_length = handlers.Length;

        if (handler_length <= 0) return default;
        if (handler_length == 1) return handlers[0].Invoke(notification, cancellationToken);

        var tasks = new Task[handler_length];
        for (int i = 0; i < handler_length; i++)
            tasks[i] = handlers[i].Invoke(notification, cancellationToken).AsTask();

        return new ValueTask(Task.WhenAll(tasks));
    }
}


internal static class RequestHandlerSlot<TRequest> where TRequest : IRequest
{
    private static readonly Type _serviceType = typeof(IRequestHandler<TRequest>);
    public delegate ValueTask Delegate(IServiceProvider provider, TRequest request, CancellationToken cancellationToken);

    public static Delegate Handler = async (provider, request, ct) =>
    {
        //var handler = provider.GetRequiredService<IRequestHandler<TRequest>>();
        var handler = (IRequestHandler<TRequest>)provider.GetService(_serviceType)
            ?? throw new InvalidOperationException($"未找到请求处理器: {typeof(IRequestHandler<TRequest>)}");

        await handler.HandleAsync(request, ct);
    };
}

internal static class RequestHandlerSlot<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private static readonly Type _serviceType = typeof(IRequestHandler<TRequest, TResponse>);
    public delegate ValueTask<TResponse> Delegate(IServiceProvider provider, TRequest command, CancellationToken cancellationToken);

    public static Delegate Handler = async (provider, command, ct) =>
    {
        //var handler = provider.GetRequiredService<IRequestHandler<TRequest, TResponse>>();
        var handler = (IRequestHandler<TRequest, TResponse>)provider.GetService(_serviceType)
            ?? throw new InvalidOperationException($"未找到请求响应处理器: {typeof(IRequestHandler<TRequest, TResponse>)}");

        return await handler.HandleAsync(command, ct);
    };
}

internal static class NotificationHandlerSlot<TNotification> where TNotification : INotification
{
    private static readonly Type _serviceType = typeof(IEnumerable<INotificationHandler<TNotification>>);
    private static readonly ConditionalWeakTable<IServiceProvider, Delegate[]> _handlerMap = new();

    public delegate ValueTask Delegate(TNotification notification, CancellationToken cancellationToken);

    public static Delegate[] GetHandlers(IServiceProvider provider)
    {
        return _handlerMap.GetValue(provider, sp =>
        {
            var services = (IEnumerable<INotificationHandler<TNotification>>)sp.GetService(_serviceType) ?? [];
            return [.. services.Select(handler => new Delegate(handler.HandleAsync))];
        });
    }
}