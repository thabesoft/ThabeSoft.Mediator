namespace ThabeSoft.Mediator.DependencyInjection;



public class DescriptorBase : IServiceTypeDescriptable
{
    public Type ServiceType { get; }
    public Type ImplementationType { get; }
    public LifetimeKind Lifetime { get; set; }


    protected DescriptorBase(
        MiddlewareDescriptorCollection root,
        HandlerKind kind,
        Type interfaceType,
        Type implementationType,
        Type inputType,
        Type? responseType)
    {
        _root = root;
        HandlerKind = kind;
        ServiceType = interfaceType;
        ImplementationType = implementationType;

        InputType = inputType;
        OutputType = responseType;
    }


    /// <summary>
    /// 排除自己
    /// </summary>
    /// <returns></returns>
    public IMiddlewareDescriptorCollection Except()
    {
        _root.ExceptAll(x => x.Equals(this));
        return _root;
    }

    /// <summary>
    /// 返回上一级
    /// </summary>
    /// <returns></returns>
    public IMiddlewareDescriptorCollection Back()
    {
        return _root;
    }
}



/// <summary>
/// 中间件描述器
/// </summary>
public sealed class MiddlewareDescriptor : IMiddlewareDescriptor
{
    private readonly MiddlewareDescriptorCollection _root;


    public HandlerKind HandlerKind { get; }
    public Type ServiceType { get; }
    public Type ImplementationType { get; }
    public Type InputType { get; }
    public Type? OutputType { get;  }
    public LifetimeKind Lifetime { get; set; }

    

    private MiddlewareDescriptor(
        MiddlewareDescriptorCollection root,
        HandlerKind kind, 
        Type interfaceType, 
        Type implementationType, 
        Type inputType, 
        Type? responseType)
    {
        _root = root;
        HandlerKind = kind;
        ServiceType = interfaceType;
        ImplementationType = implementationType;

        InputType = inputType;
        OutputType = responseType;
    }

    public static MiddlewareDescriptor Request<TMiddleware, TInput, TOutput>(MiddlewareDescriptorCollection root)
        where TMiddleware : IMiddleware<TInput, TOutput>
        where TInput : IRequest<TOutput>
    {
        return new MiddlewareDescriptor(root,
            HandlerKind.RequestResponse, 
            typeof(IMiddleware<TInput, TOutput>), 
            typeof(TMiddleware), 
            typeof(TInput), 
            typeof(TOutput));
    }
    public static MiddlewareDescriptor Request<TMiddleware, TInput>(MiddlewareDescriptorCollection root)
        where TMiddleware : IMiddleware<TInput>
        where TInput : IRequest
    {
        return new MiddlewareDescriptor(root,
            HandlerKind.Request, 
            typeof(IMiddleware<TInput>),
            typeof(TMiddleware),
            typeof(TInput), 
            null);
    }

    /// <summary>
    /// 排除自己
    /// </summary>
    /// <returns></returns>
    public IMiddlewareDescriptorCollection Except()
    {
        _root.ExceptAll(x => x.Equals(this));
        return _root;
    }

    /// <summary>
    /// 返回上一级
    /// </summary>
    /// <returns></returns>
    public IMiddlewareDescriptorCollection Back()
    {
        return _root;
    }

    
    public override bool Equals(object obj)
    {
        if (ReferenceEquals(this, obj)) return true;
        if (obj is not IMiddlewareDescriptor other) return false;

        return Equals(other);
    }
    /// <summary>
    ///  如果 <see cref="ServiceType"/> 相同就认定是一个对象
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public bool Equals(IMiddlewareDescriptor other)
    {
        return ServiceType == other.ServiceType && ImplementationType == other.ImplementationType;
    }

    /// <summary>
    /// 返回 <see cref="ServiceType"/> 的 HashCode
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
        return $"[{HandlerKind}] {ServiceType}, {ImplementationType}";
    }
}