using Microsoft.Extensions.DependencyInjection;

namespace ThabeSoft.Mediator.DependencyInjection;

/// <summary>
/// 业务描述集合
/// </summary>
public interface IDescriptorCollection
{
    /// <summary>
    /// 默认生命周期
    /// </summary>
    ServiceLifetime DefaultLifetime { get; }

    /// <summary>
    /// 设置默认生命周期类型
    /// </summary>
    /// <param name="lifetime"></param>
    /// <returns></returns>
    IDescriptorCollection Default(ServiceLifetime lifetime);

    /// <summary>
    /// 查询符合条件的元素进行批处理
    /// </summary>
    /// <param name="matcher"></param>
    /// <returns></returns>
    IDescriptorBatch Batch(Func<IDescriptorBuilder, bool> matcher);


    /// <summary>
    /// 添加请求
    /// </summary>
    /// <typeparam name="THandler">处理器类型</typeparam>
    /// <typeparam name="TRequest">请求类型</typeparam>
    /// <typeparam name="TResponse">响应类型</typeparam>
    /// <returns></returns>
    IDescriptorBuilder AddRequestHandler<THandler, TRequest, TResponse>()
      where THandler : IRequestHandler<TRequest, TResponse>
      where TRequest : IRequest<TResponse>;

    /// <summary>
    /// 添加请求处理器
    /// </summary>
    /// <typeparam name="THandler">处理器类型</typeparam>
    /// <typeparam name="TRequest">请求类型</typeparam>
    /// <returns></returns>
    IDescriptorBuilder AddRequestHandler<THandler, TRequest>()
       where THandler : IRequestHandler<TRequest>
       where TRequest : IRequest;

    /// <summary>
    /// 添加通知处理器
    /// </summary>
    /// <typeparam name="THandler">处理器类型</typeparam>
    /// <typeparam name="TNotification">通知类型</typeparam>
    /// <returns></returns>
    IDescriptorBuilder AddNotificationHandler<THandler, TNotification>()
       where THandler : INotificationHandler<TNotification>
       where TNotification : INotification;

    /// <summary>
    /// 添加请求管道行为
    /// </summary>
    /// <typeparam name="TBehavior">管道行为实现类型</typeparam>
    /// <typeparam name="TRequest">请求类型</typeparam>
    /// <typeparam name="TResponse">响应类型</typeparam>
    /// <returns></returns>
    IDescriptorBuilder AddRequestBehavior<TBehavior, TRequest, TResponse>()
       where TBehavior : IRequestPipelineBehavior<TRequest, TResponse>
       where TRequest : IRequest<TResponse>;

    /// <summary>
    /// 添加请求管道行为
    /// </summary>
    /// <typeparam name="TBehavior">管道行为实现类型</typeparam>
    /// <typeparam name="TRequest">请求类型</typeparam>
    /// <returns></returns>
    IDescriptorBuilder AddRequestBehavior<TBehavior, TRequest>()
       where TBehavior : IRequestPipelineBehavior<TRequest>
       where TRequest : IRequest;

    /// <summary>
    /// 添加通知管道行为
    /// </summary>
    /// <typeparam name="TBehavior">管道行为实现类型</typeparam>
    /// <typeparam name="TNotification">通知类型</typeparam>
    /// <returns></returns>
    public IDescriptorBuilder AddNotificationBehavior<TBehavior, TNotification>()
      where TBehavior : INotificationPipelineBehavior<TNotification>
      where TNotification : INotification;
}

/// <summary>
/// 业务描述集合扩展
/// </summary>
public static class DescriptorCollectionExtensions
{
    extension(IDescriptorCollection collection)
    {
        /// <summary>
        /// 根据生命周期查询
        /// </summary>
        public IDescriptorBatch WithLifetime(LifetimeKind lifetime)
        {
            return collection.Batch(x => lifetime.HasFlag(x.Lifetime));
        }

        /// <summary>
        /// 所有单例的
        /// </summary>
        public IDescriptorBatch Singleton()
        {
            return collection.WithLifetime(LifetimeKind.Singleton);
        }
        /// <summary>
        /// 所有作用域的
        /// </summary>
        public IDescriptorBatch Scoped()
        {
            return collection.WithLifetime(LifetimeKind.Scoped);
        }
        /// <summary>
        /// 所有瞬态的
        /// </summary>
        public IDescriptorBatch Transient()
        {
            return collection.WithLifetime(LifetimeKind.Transient);
        }
        /// <summary>
        /// 所有未指定生命周期的
        /// </summary>
        public IDescriptorBatch None()
        {
            return collection.WithLifetime(LifetimeKind.None);
        }

        /// <summary>
        /// 所有的
        /// </summary>
        public IDescriptorBatch All()
        {
            return collection.Batch(_ => true);
        }

        /// <summary>
        /// 查询该请求的所有处理器
        /// </summary>
        public IDescriptorBatch RequestHandler<TRequest>()
            where TRequest : IRequest
        {
            var service_type = typeof(IRequestHandler<TRequest>);
            return collection.Batch(x => x.ServiceType == service_type);
        }

        /// <summary>
        /// 查询该请求的所有处理器
        /// </summary>
        public IDescriptorBatch RequestHandler<TRequest, TResult>()
            where TRequest : IRequest<TResult>
        {
            var service_type = typeof(IRequestHandler<TRequest, TResult>);
            return collection.Batch(x => x.ServiceType == service_type);
        }

        /// <summary>
        /// 查询该通知的所有处理器
        /// </summary>
        public IDescriptorBatch NotificationHandler<TNotification>()
           where TNotification : INotification
        {
            var service_type = typeof(INotificationHandler<TNotification>);
            return collection.Batch(x => x.ServiceType == service_type);
        }



        /// <summary>
        /// 所有请求处理器
        /// </summary>
        public IDescriptorBatch RequestHandler(bool includeResponseRequest = true)
        {
            if (includeResponseRequest)
            {
                return collection.Batch(x => x.HandlerKind == HandlerKind.Request || x.HandlerKind == HandlerKind.RequestResponse);
            }
            else
            {
                return collection.Batch(x => x.HandlerKind == HandlerKind.Request);
            }
        }

        /// <summary>
        /// 所有通知处理器
        /// </summary>
        public IDescriptorBatch NotificationHandler()
        {
            return collection.Batch(x => x.HandlerKind == HandlerKind.Notification);
        }

        /// <summary>
        /// 所有处理器
        /// </summary>
        public IDescriptorBatch Handler()
        {
            return collection.Batch(x => x.Kind == DescriptorKind.Handler);
        }

        /// <summary>
        /// 所有管道行为
        /// </summary>
        public IDescriptorBatch Behavior()
        {
            return collection.Batch(x => x.Kind == DescriptorKind.Behavior);
        }
    }
}
