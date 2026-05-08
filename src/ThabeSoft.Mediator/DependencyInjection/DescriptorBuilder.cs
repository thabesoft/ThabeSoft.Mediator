namespace ThabeSoft.Mediator.DependencyInjection;


/// <summary>
/// 描述构建器基类
/// </summary>
public sealed class DescriptorBuilder : IDescriptorBuilder
{
    private readonly DescriptorCollection root;

    private DescriptorBuilder(
        DescriptorCollection root,
        Type serviceType,
        Type implementationType,
        HandlerKind kind,
        Type messageType,
        Type? messageResponseType = null
        )
    {
        this.root = root;
        ServiceType = serviceType;
        ImplementationType = implementationType;
        HandlerKind = kind;
        InputType = messageType;
        OutputType = messageResponseType;
    }

    public Type ServiceType { get; }
    public Type ImplementationType { get; }
    public HandlerKind HandlerKind { get; }
    public Type InputType { get; }
    public Type? OutputType { get; }
    public LifetimeKind Lifetime { get; private set; }


    public static DescriptorBuilder Request<THandler, TRequest>(DescriptorCollection root)
        where THandler : IRequestHandler<TRequest>
        where TRequest : IRequest
    {
        return new DescriptorBuilder(root, typeof(IRequestHandler<TRequest>), typeof(THandler), HandlerKind.Request, typeof(TRequest));
    }
    public static DescriptorBuilder Request<THandler, TRequest, TResponse>(DescriptorCollection root)
        where THandler : IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        return new DescriptorBuilder(root, typeof(IRequestHandler<TRequest, TResponse>), typeof(THandler), HandlerKind.RequestResponse, typeof(TRequest), typeof(TResponse));
    }
    public static DescriptorBuilder Notification<THandler, TNotification>(DescriptorCollection root)
        where THandler : INotificationHandler<TNotification>
        where TNotification : INotification
    {
        return new DescriptorBuilder(root, typeof(INotificationHandler<TNotification>), typeof(THandler), HandlerKind.Notification, typeof(TNotification));
    }


    public static DescriptorBuilder RequestMiddlewar<THandler, TRequest>(DescriptorCollection root)
        where THandler : IMiddleware<TRequest>
        where TRequest : IRequest
    {
        return new DescriptorBuilder(root, typeof(IMiddleware<TRequest>), typeof(THandler), HandlerKind.Request, typeof(TRequest));
    }
    public static DescriptorBuilder RequestMiddlewar<THandler, TRequest, TResponse>(DescriptorCollection root)
        where THandler : IMiddleware<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        return new DescriptorBuilder(root, typeof(IMiddleware<TRequest, TResponse>), typeof(THandler), HandlerKind.RequestResponse, typeof(TRequest), typeof(TResponse));
    }



    /// <summary>
    /// 设置生命周期
    /// </summary>
    /// <param name="lifetime"></param>
    /// <returns></returns>
    public IDescriptorBuilder SetLifetime(LifetimeKind lifetime)
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
        root.ExceptAll(x => x.Equals(this));
        return root;
    }
    /// <summary>
    /// 返回上一级
    /// </summary>
    /// <returns></returns>
    public IDescriptorCollection Back()
    {
        return root;
    }


    /// <summary>
    ///  如果 <see cref="ServiceType"/> 和 <see cref="ImplementationType"/> 相同就认定是一个对象
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object obj)
    {
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
        if (Lifetime == LifetimeKind.None)
        {
            return $"{ServiceType}[{Lifetime}]";
        }

        return $"{ServiceType}";
    }
}