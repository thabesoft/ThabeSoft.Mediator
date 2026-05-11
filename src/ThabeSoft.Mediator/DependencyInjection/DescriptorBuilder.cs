using Microsoft.Extensions.DependencyInjection;

namespace ThabeSoft.Mediator.DependencyInjection;


/// <summary>
/// 描述构建器基类
/// </summary>
public sealed class DescriptorBuilder : IDescriptorBuilder
{
    private readonly DescriptorCollection _root;

    private DescriptorBuilder(
        DescriptorCollection root,
        DescriptorKind kind,
        Type serviceType,
        Type implementationType,
        HandlerKind handlerKind,
        Type messageType,
        Type? messageResponseType = null
        )
    {
        _root = root;
        Kind = kind;
        ServiceType = serviceType;
        ImplementationType = implementationType;
        HandlerKind = handlerKind;
        InputType = messageType;
        OutputType = messageResponseType;
    }

    public DescriptorKind Kind { get; }
    public Type ServiceType { get; }
    public Type ImplementationType { get; }
    public HandlerKind HandlerKind { get; }
    public Type InputType { get; }
    public Type? OutputType { get; }
    public ServiceLifetime? Lifetime { get; private set; }


    public static DescriptorBuilder RequestHandler<THandler, TRequest>(DescriptorCollection root)
        where THandler : IRequestHandler<TRequest>
        where TRequest : IRequest
    {
        return new DescriptorBuilder(root, DescriptorKind.Handler, typeof(IRequestHandler<TRequest>), typeof(THandler), HandlerKind.Request, typeof(TRequest));
    }
    public static DescriptorBuilder RequestHandler<THandler, TRequest, TResponse>(DescriptorCollection root)
        where THandler : IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        return new DescriptorBuilder(root, DescriptorKind.Handler, typeof(IRequestHandler<TRequest, TResponse>), typeof(THandler), HandlerKind.RequestResponse, typeof(TRequest), typeof(TResponse));
    }
    public static DescriptorBuilder NotificationHandler<THandler, TNotification>(DescriptorCollection root)
        where THandler : INotificationHandler<TNotification>
        where TNotification : INotification
    {
        return new DescriptorBuilder(root, DescriptorKind.Handler, typeof(INotificationHandler<TNotification>), typeof(THandler), HandlerKind.Notification, typeof(TNotification));
    }


    public static DescriptorBuilder RequestBehavior<TBehavior, TRequest>(DescriptorCollection root)
        where TBehavior : IRequestPipelineBehavior<TRequest>
        where TRequest : IRequest
    {
        return new DescriptorBuilder(root, DescriptorKind.PipelineBehavior, typeof(IRequestPipelineBehavior<TRequest>), typeof(TBehavior), HandlerKind.Request, typeof(TRequest));
    }
    public static DescriptorBuilder RequestBehavior<TBehavior, TRequest, TResponse>(DescriptorCollection root)
        where TBehavior : IRequestPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        return new DescriptorBuilder(root, DescriptorKind.PipelineBehavior, typeof(IRequestPipelineBehavior<TRequest, TResponse>), typeof(TBehavior), HandlerKind.RequestResponse, typeof(TRequest), typeof(TResponse));
    }
    public static DescriptorBuilder NotificationBehavior<TBehavior, TNotification>(DescriptorCollection root)
        where TBehavior : INotificationPipelineBehavior<TNotification>
        where TNotification : INotification
    {
        return new DescriptorBuilder(root, DescriptorKind.PipelineBehavior, typeof(INotificationPipelineBehavior<TNotification>), typeof(TBehavior), HandlerKind.Notification, typeof(TNotification));
    }



    /// <summary>
    /// 设置生命周期
    /// </summary>
    /// <param name="lifetime"></param>
    /// <returns></returns>
    public IDescriptorBuilder SetLifetime(ServiceLifetime? lifetime)
    {
        Lifetime = lifetime;
        return this;
    }
    /// <summary>
    /// 排除自己
    /// </summary>
    /// <returns></returns>
    public IDescriptorCollection Except()
    {
        _root.ExceptAll(x => x.Equals(this));
        return _root;
    }
    /// <summary>
    /// 返回上一级
    /// </summary>
    /// <returns></returns>
    public IDescriptorCollection Back()
    {
        return _root;
    }


    /// <summary>
    ///  如果 <see cref="ServiceType"/> 和 <see cref="ImplementationType"/> 相同就认定是一个对象
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj is not IDescriptorBuilder other) return false;

        return ServiceType == other.ServiceType && ImplementationType == other.ImplementationType;
    }

    /// <summary>
    /// 返回 (<see cref="ServiceType"/>, <see cref="ImplementationType"/>) 的 HashCode
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        return (ServiceType, ImplementationType).GetHashCode();
    }

    /// <summary>
    /// HandlerType[Lifetime]
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        if (Lifetime is null)
        {
            return $"{ServiceType}[{Lifetime}]";
        }

        return $"{ServiceType}";
    }
}