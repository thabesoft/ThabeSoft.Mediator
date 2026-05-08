namespace ThabeSoft.Mediator.DependencyInjection;


/// <summary>
/// 处理器描述集合
/// </summary>
public interface IMiddlewareDescriptorCollection : 
    IDescriptorCollection<
        IMiddlewareDescriptorCollection, 
        IMiddlewareDescriptorBatch, 
        IMiddlewareDescriptor>
{
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