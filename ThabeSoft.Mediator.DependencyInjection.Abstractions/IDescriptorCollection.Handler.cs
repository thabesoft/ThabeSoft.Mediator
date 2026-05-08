namespace ThabeSoft.Mediator.DependencyInjection;

/// <summary>
/// 处理器描述集合
/// </summary>
public interface IHandlerDescriptorCollection :  
    IDescriptorCollection<
        IHandlerDescriptorCollection, 
        IHandlerDescriptorBatch, 
        IHandlerDescriptor>
{
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
}