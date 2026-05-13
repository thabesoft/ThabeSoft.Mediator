using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace ThabeSoft.Mediator;


public static class PublisherExtensions
{
    private static readonly MethodInfo _notificationMethod;

    static PublisherExtensions()
    {
        var publisher_methods = typeof(IPublisher)
            .GetMethods()
            .Where(x => x.Name == nameof(IPublisher.PublishAsync) && x.IsGenericMethod)
            .ToArray();

        // ValueTask SendAsync<TRequest>(TRequest request, CancellationToken cancellationToken);
        _notificationMethod = publisher_methods.First(x =>
        {
            var @params = x.GetParameters();
            if (@params.Length != 2) return false;

            if (!@params[0].ParameterType.IsGenericParameter) return false;
            if (@params[1].ParameterType != typeof(CancellationToken)) return false;
            if (x.ReturnType != typeof(ValueTask)) return false;

            return true;
        });
    }


    extension(IPublisher publisher)
    {
        public ValueTask PublishUntypedAsync(INotification notification, CancellationToken cancellationToken = default)
        {
            if (notification is null) throw new ArgumentNullException(nameof(notification), "通知不可为空");

            var actual_notification_type = notification.GetType();

            var invoker = NotificationCahce.Delegates.GetOrAdd(actual_notification_type, type =>
            {
                // 获取方法信息
                var genericMethod = _notificationMethod.MakeGenericMethod(type);

                // 构建
                var publisher_param = Expression.Parameter(typeof(IPublisher), "publisher");
                var notification_param = Expression.Parameter(typeof(object), "notification");
                var ct_param = Expression.Parameter(typeof(CancellationToken), "ct");

                // 转为实际请求类型
                var convertedRequest = Expression.Convert(notification_param, type);
                // 构建委托
                var call = Expression.Call(publisher_param, genericMethod, convertedRequest, ct_param);
                var lambda = Expression.Lambda<NotificationCahce.Delegate>(call, publisher_param, notification_param, ct_param);
                return lambda.Compile();
            });

            return invoker.Invoke(publisher, notification, cancellationToken);
        }
    }

    private static class NotificationCahce
    {
        public delegate ValueTask Delegate(IPublisher publisher, object notification, CancellationToken ct);
        public static readonly ConcurrentDictionary<Type, Delegate> Delegates = new();
    }
}