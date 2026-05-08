using Microsoft.Extensions.DependencyInjection;


namespace ThabeSoft.Mediator.DependencyInjection;

/// <summary>
/// 处理器描述集合
/// </summary>
public interface IMiddlewareDescriptorCollection
{
    public ServiceLifetime DefaultLifetime { get; }


    /// <summary>
    /// 设置默认生命周期
    /// </summary>
    /// <param name="lifetime"></param>
    /// <returns></returns>
    IMiddlewareDescriptorCollection SetDefaultLifetime(ServiceLifetime lifetime);

    /// <summary>
    /// 查询所有符合条件的并更新
    /// </summary>
    /// <param name="matcher"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    IMiddlewareDescriptorCollection UpdateAll(Func<IMiddlewareDescriptor, bool> matcher, Action<IMiddlewareDescriptor> action);

    /// <summary>
    /// 过滤所有符合条件的处理器
    /// </summary>
    /// <param name="matcher"></param>
    /// <returns></returns>
    IMiddlewareDescriptorCollection ExceptAll(Func<IMiddlewareDescriptor, bool> matcher);

    /// <summary>
    /// 批处理
    /// </summary>
    /// <param name="matcher"></param>
    /// <returns></returns>
    IMiddlewareDescriptorBatch FindAll(Func<IMiddlewareDescriptor, bool> matcher);


    /// <summary>
    /// 添加请求响应
    /// </summary>
    /// <typeparam name="TMiddleware"></typeparam>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <returns></returns>
    public IMiddlewareDescriptor AddRequest<TMiddleware, TRequest, TResponse>()
       where TMiddleware : IMiddleware<TRequest, TResponse>
       where TRequest : IRequest<TResponse>;

    /// <summary>
    /// 添加请求
    /// </summary>
    /// <typeparam name="TMiddleware"></typeparam>
    /// <typeparam name="TRequest"></typeparam>
    /// <returns></returns>
    public IMiddlewareDescriptor AddRequest<TMiddleware, TRequest>()
        where TMiddleware : IMiddleware<TRequest>
        where TRequest : IRequest;
}



/// <summary>
/// 扩展方法
/// </summary>
public static class MiddlewareDescriptorCollectionExtensions
{
    // 硬编码 API
    extension(IMiddlewareDescriptorCollection collection)
    {
        public IMiddlewareDescriptorBatch All()
        {
            return collection.FindAll(x => true);
        }


        /// <summary>
        /// 根据类型查询
        /// </summary>
        /// <param name="kind"></param>
        /// <returns></returns>
        public IMiddlewareDescriptorBatch OfKind(MiddlewareKind kind)
        {
            return collection.FindAll(x => x.Kind == kind);
        }
        public IMiddlewareDescriptorBatch Closed()
        {
            return collection.FindAll(x => x.Kind == MiddlewareKind.Closed);
        }
        public IMiddlewareDescriptorBatch Open()
        {
            return collection.FindAll(x => x.Kind == MiddlewareKind.Open);
        }
    }
}