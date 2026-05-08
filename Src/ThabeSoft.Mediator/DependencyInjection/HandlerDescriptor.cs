namespace ThabeSoft.Mediator.DependencyInjection;


/// <summary>
/// 处理器描述器
/// </summary>
public sealed class HandlerDescriptor : DescriptorBuilderBase<HandlerDescriptor, HandlerDescriptorCollection, HandlerDescriptorBatch>
    private HandlerDescriptor(
        HandlerDescriptorCollection root,
        Type serviceType,
        Type implementationType, 
        HandlerKind kind, 
        Type inputType,
        Type? outputType = null) : base(root, serviceType, implementationType, kind, inputType, outputType)
    {

    }

    public static HandlerDescriptor Request<THandler, TRequest>(HandlerDescriptorCollection root)
        where THandler : IRequestHandler<TRequest>
        where TRequest : IRequest
    {
        return new HandlerDescriptor(root, typeof(IRequestHandler<TRequest>), typeof(THandler), HandlerKind.Request, typeof(TRequest));
    }
    public static HandlerDescriptor Request<THandler, TRequest, TResponse>(HandlerDescriptorCollection root)
        where THandler : IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        return new HandlerDescriptor(root, typeof(IRequestHandler<TRequest, TResponse>), typeof(THandler), HandlerKind.RequestResponse, typeof(TRequest), typeof(TResponse));
    }
    public static HandlerDescriptor Notification<THandler, TNotification>(HandlerDescriptorCollection root)
        where THandler : INotificationHandler<TNotification>
        where TNotification : INotification
    {
        return new HandlerDescriptor(root, typeof(INotificationHandler<TNotification>), typeof(THandler), HandlerKind.Notification, typeof(TNotification));
    }

    protected override HandlerDescriptor This() => this;
}