using Microsoft.Extensions.DependencyInjection;

namespace ThabeSoft.Mediator.DependencyInjection;


/// <summary>
/// 中间件描述器
/// </summary>
public sealed class MiddlewareDescriptor : IMiddlewareDescriptor
{
    private readonly IMiddlewareDescriptorCollection _root;

    public MiddlewareKind Kind { get; }
    public Type ServiceType { get; }
    public Type ImplementationType { get; }
    public Type? InputType { get;  }
    public Type? OutputType { get;  }
    public ServiceLifetime? Lifetime { get; private set; }


    private MiddlewareDescriptor(
        IMiddlewareDescriptorCollection root, 
        MiddlewareKind kind, 
        Type interfaceType, 
        Type implementationType, 
        Type inputType, 
        Type? responseType)
    {
        _root = root;
        Kind = kind;
        ServiceType = interfaceType;
        ImplementationType = implementationType;

        InputType = inputType;
        OutputType = responseType;
    }

    public static MiddlewareDescriptor Request<TMiddleware, TInput, TOutput>(IMiddlewareDescriptorCollection root)
        where TMiddleware : IMiddleware<TInput, TOutput>
        where TInput : IRequest<TOutput>
    {
        return new MiddlewareDescriptor(root, 
            MiddlewareKind.Closed, 
            typeof(IMiddleware<TInput, TOutput>), 
            typeof(TMiddleware), 
            typeof(TInput), 
            typeof(TOutput));
    }
    public static MiddlewareDescriptor Request<TMiddleware, TInput>(IMiddlewareDescriptorCollection root)
        where TMiddleware : IMiddleware<TInput>
        where TInput : IRequest
    {
        return new MiddlewareDescriptor(root, 
            MiddlewareKind.Closed, 
            typeof(IMiddleware<TInput>),
            typeof(TMiddleware),
            typeof(TInput), 
            null);
    }


    /// <summary>
    /// 设置生命周期
    /// </summary>
    /// <param name="lifetime"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public IMiddlewareDescriptor SetLifetime(ServiceLifetime lifetime)
    {
        if (lifetime is ServiceLifetime.Scoped or ServiceLifetime.Singleton or ServiceLifetime.Transient)
        {
            Lifetime = lifetime;
            return this;
        }

        throw new ArgumentException($"生命周期类型异常: {lifetime}");
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


    public static bool operator ==(MiddlewareDescriptor left, MiddlewareDescriptor rigt) => Equals(left, rigt);
    public static bool operator !=(MiddlewareDescriptor left, MiddlewareDescriptor rigt) => !Equals(left, rigt);


    
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
        return $"[{Kind}] {ServiceType}, {ImplementationType}";
    }
}