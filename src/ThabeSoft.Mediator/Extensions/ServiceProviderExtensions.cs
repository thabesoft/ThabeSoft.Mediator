using Microsoft.Extensions.DependencyInjection;

namespace ThabeSoft.Mediator.Extensions;


/// <summary>
/// 业务容器扩展
/// </summary>
public static class ServiceProviderExtensions
{
    /// <summary>
    /// 请求响应扩展
    /// </summary>
    /// <typeparam name="TRequest">请求类型</typeparam>
    /// <typeparam name="TResponse">响应类型</typeparam>
    /// <param name="services"></param>
    extension<TRequest, TResponse>(IServiceProvider services) where TRequest : IRequest<TResponse>
    {
        /// <summary>
        /// 获取请求响应管道
        /// </summary>
        /// <returns></returns>
        public Pipeline<TRequest, TResponse> GetRequiredRequestPipeline()
        {
            var handler = services.GetRequiredService<IRequestHandler<TRequest, TResponse>>();
            var middlewares = services.GetServices<IMiddleware<TRequest, TResponse>>();

            var middleware_delegate = MiddlewareBuilder.BuildRequest([.. middlewares]);
            return (request, ct) => middleware_delegate.Invoke(request, handler.HandleAsync, ct);
        }
    }

    /// <summary>
    /// 请求扩展
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <param name="services"></param>
    extension<TRequest>(IServiceProvider services) where TRequest : IRequest
    {
        /// <summary>
        /// 获取请求管道
        /// </summary>
        /// <returns></returns>
        public Pipeline<TRequest> GetRequiredRequestPipeline()
        {
            var handler = services.GetRequiredService<IRequestHandler<TRequest>>();
            var middlewares = services.GetServices<IMiddleware<TRequest>>();

            var middleware_delegate = MiddlewareBuilder.BuildRequest([.. middlewares]);
            return (request, ct) => middleware_delegate.Invoke(request, handler.HandleAsync, ct);
        }
    }


    /// <summary>
    /// 通知扩展
    /// </summary>
    /// <typeparam name="TNotification"></typeparam>
    /// <param name="services"></param>
    extension<TNotification>(IServiceProvider services) where TNotification : INotification
    {
        /// <summary>
        /// 获取通知处理器
        /// </summary>
        /// <typeparam name="TNotification">通知类型</typeparam>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">没有匹配的处理器</exception>
        public IEnumerable<INotificationHandler<TNotification>> GetNotificationHandlers()
        {
            return services.GetServices<INotificationHandler<TNotification>>();
        }
    }
    
}