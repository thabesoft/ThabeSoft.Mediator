using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

#if DEBUG
using System.Linq.Expressions;
#endif

namespace ThabeSoft.Mediator.DependencyInjection;

/// <summary>
/// 处理器描述集合
/// </summary>
public interface IHandlerDescriptorCollection
{
    public ServiceLifetime DefaultLifetime { get; }

    #region --行为操作--

    /// <summary>
    /// 设置处理器默认生命周期
    /// </summary>
    /// <param name="lifetime"></param>
    /// <returns></returns>
    IHandlerDescriptorCollection SetDefaultLifetime(ServiceLifetime lifetime);

    /// <summary>
    /// 查询所有符合条件的处理器并更新
    /// </summary>
    /// <param name="matcher"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    IHandlerDescriptorCollection UpdateAll(Func<HandlerDescriptor, bool> matcher, Action<HandlerDescriptor> action);

    /// <summary>
    /// 过滤所有符合条件的处理器
    /// </summary>
    /// <param name="matcher"></param>
    /// <returns></returns>
    IHandlerDescriptorCollection ExceptAll(Func<HandlerDescriptor, bool> matcher);

    /// <summary>
    /// 批处理
    /// </summary>
    /// <param name="matcher"></param>
    /// <returns></returns>
    IHandlerDescriptorBatch FindAll(Func<HandlerDescriptor, bool> matcher);

    #endregion

    #region --添加操作--

    /// <summary>
    /// 添加请求处理器
    /// </summary>
    /// <typeparam name="THandler"></typeparam>
    /// <typeparam name="TRequest"></typeparam>
    /// <returns></returns>
    IHandlerDescriptor AddRequest<THandler, TRequest>()
      where THandler : IRequestHandler<TRequest>
      where TRequest : IRequest;

    /// <summary>
    /// 添加请求-响应处理器
    /// </summary>
    /// <typeparam name="THandler"></typeparam>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <returns></returns>
    IHandlerDescriptor AddRequest<THandler, TRequest, TResponse>()
       where THandler : IRequestHandler<TRequest, TResponse>
       where TRequest : IRequest<TResponse>;

    /// <summary>
    /// 添加通知处理器
    /// </summary>
    /// <typeparam name="THandler"></typeparam>
    /// <typeparam name="TNotification"></typeparam>
    /// <returns></returns>
    IHandlerDescriptor AddNotification<THandler, TNotification>()
       where THandler : INotificationHandler<TNotification>
       where TNotification : INotification;

    #endregion
}


/// <summary>
/// 扩展方法
/// </summary>
public static class HandlerDescriptorCollectionExtensions
{
#if DEBUG
    extension(IHandlerDescriptorCollection collection)
    {
        /// <summary>
        /// 查询所有符合条件的处理器并更新
        /// </summary>
        /// <param name="matcher"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public IHandlerDescriptorCollection UpdateAll(Expression<Func<HandlerDescriptor, bool>> matcher, Expression<Action<HandlerDescriptor>> action)
        {
            Debug.WriteLine($"更新条件", matcher.ToString());
            Debug.WriteLine($"更新语句", action.ToString());

            var matcher_method = matcher.Compile();
            var action_method = action.Compile();

            return collection.UpdateAll(matcher_method, action_method);
        }

        /// <summary>
        /// 过滤所有符合条件的处理器
        /// </summary>
        /// <param name="matcher"></param>
        /// <returns></returns>
        public IHandlerDescriptorCollection ExceptAll(Expression<Func<HandlerDescriptor, bool>> matcher)
        {
            Debug.WriteLine($"排除条件", matcher.ToString());
            var matcher_method = matcher.Compile();

            return collection.ExceptAll(matcher_method);
        }

        /// <summary>
        /// 批处理
        /// </summary>
        /// <param name="matcher"></param>
        /// <returns></returns>
        public IHandlerDescriptorBatch FindAll(Expression<Func<HandlerDescriptor, bool>> matcher)
        {
            Debug.WriteLine($"查询条件", matcher.ToString());
            var matcher_method = matcher.Compile();

            return collection.FindAll(matcher_method);
        }
    }
#endif

    // 硬编码 API
    extension(IHandlerDescriptorCollection collection)
    {
        public IHandlerDescriptorBatch FindAllByRequest(bool includeResponseRequest = true)
        {
            if (includeResponseRequest)
            {
                return collection.FindAll(x => x.Kind == HandlerKind.Request || x.Kind == HandlerKind.RequestResponse);
            }
            else
            {
                return collection.FindAll(x => x.Kind == HandlerKind.Request);
            }
        }

        public IHandlerDescriptorBatch FindAllByNotification()
        {
            return collection.FindAll(x => x.Kind == HandlerKind.Notification);
        }


        /// <summary>
        /// 查询该请求的所有处理器
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <returns></returns>
        public IHandlerDescriptorBatch FindAllByRequest<TRequest>()
            where TRequest : IRequest
        {
            var service_type = typeof(IRequestHandler<TRequest>);
            return collection.FindAll(x => x.ServiceType == service_type);
        }

        /// <summary>
        /// 查询该请求的所有处理器
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public IHandlerDescriptorBatch FindAllByRequest<TRequest, TResult>()
            where TRequest : IRequest<TResult>
        {
            var service_type = typeof(IRequestHandler<TRequest, TResult>);
            return collection.FindAll(x => x.ServiceType == service_type);
        }

        /// <summary>
        /// 查询该通知的所有处理器
        /// </summary>
        /// <typeparam name="TNotification"></typeparam>
        /// <returns></returns>
        public IHandlerDescriptorBatch FindAllByNotification<TNotification>()
           where TNotification : INotification
        {
            var service_type = typeof(INotificationHandler<TNotification>);
            return collection.FindAll(x => x.ServiceType == service_type);
        }
    }
}