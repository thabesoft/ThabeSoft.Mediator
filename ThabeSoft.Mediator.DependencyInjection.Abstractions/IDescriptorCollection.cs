using Microsoft.Extensions.DependencyInjection;

namespace ThabeSoft.Mediator.DependencyInjection;

/// <summary>
/// 业务描述集合
/// </summary>
public interface IDescriptorCollection<TSelf, TBatch, TDescriptor>
    where TDescriptor : notnull
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
    TSelf Default(ServiceLifetime lifetime);

    /// <summary>
    /// 查询符合条件的元素进行批处理
    /// </summary>
    /// <param name="matcher"></param>
    /// <returns></returns>
    TBatch Batch(Func<TDescriptor, bool> matcher);
}

/// <summary>
/// 业务描述集合扩展
/// </summary>
public static class DescriptorCollectionExtensions
{
    extension<TCollection, TBatch, TDescriptor>(IDescriptorCollection<TCollection, TBatch, TDescriptor> collection)
        where TDescriptor : IDescriptorBuilder<TDescriptor, TCollection>
    {
        /// <summary>
        /// 根据生命周期查询
        /// </summary>
        /// <param name="lifetime"></param>
        /// <returns></returns>
        public TBatch WithLifetime(LifetimeKind lifetime)
        {
            return collection.Batch(x => x.Lifetime.HasFlag(lifetime));
        }

        /// <summary>
        /// 所有单例的
        /// </summary>
        /// <returns></returns>
        public TBatch Singleton()
        {
            return collection.WithLifetime(LifetimeKind.Singleton);
        }
        /// <summary>
        /// 所有作用域的
        /// </summary>
        /// <returns></returns>
        public TBatch Scoped()
        {
            return collection.WithLifetime(LifetimeKind.Scoped);
        }
        /// <summary>
        /// 所有瞬态的
        /// </summary>
        /// <returns></returns>
        public TBatch Transient()
        {
            return collection.WithLifetime(LifetimeKind.Transient);
        }
        /// <summary>
        /// 所有未指定生命周期的
        /// </summary>
        /// <returns></returns>
        public TBatch None()
        {
            return collection.WithLifetime(LifetimeKind.None);
        }

        /// <summary>
        /// 所有的
        /// </summary>
        /// <returns></returns>
        public TBatch All()
        {
            return collection.Batch(x => true);
        }
    }






    extension<TCollection, TBatch, TDescriptor>(IDescriptorCollection<TCollection, TBatch, TDescriptor> collection)
        where TCollection : IDescriptorCollection<TCollection, TBatch, TDescriptor>
        where TDescriptor : IDescriptorBuilder<TDescriptor, TCollection>
    {
        /// <summary>
        /// 查询该请求的所有处理器
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <returns></returns>
        public TBatch Request<TRequest>()
            where TRequest : IRequest
        {
            var service_type = typeof(IRequestHandler<TRequest>);
            return collection.Batch(x => x.ServiceType == service_type);
        }

        /// <summary>
        /// 查询该请求的所有处理器
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public TBatch Request<TRequest, TResult>()
            where TRequest : IRequest<TResult>
        {
            var service_type = typeof(IRequestHandler<TRequest, TResult>);
            return collection.Batch(x => x.ServiceType == service_type);
        }

        /// <summary>
        /// 查询该通知的所有处理器
        /// </summary>
        /// <typeparam name="TNotification"></typeparam>
        /// <returns></returns>
        public TBatch Notifications<TNotification>()
           where TNotification : INotification
        {
            var service_type = typeof(INotificationHandler<TNotification>);
            return collection.Batch(x => x.ServiceType == service_type);
        }



        /// <summary>
        /// 所有请求处理器
        /// </summary>
        /// <param name="includeResponseRequest"></param>
        /// <returns></returns>
        public TBatch Requests(bool includeResponseRequest = true)
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
        /// <returns></returns>
        public TBatch Notifications()
        {
            return collection.Batch(x => x.HandlerKind == HandlerKind.Notification);
        }
    }
}
