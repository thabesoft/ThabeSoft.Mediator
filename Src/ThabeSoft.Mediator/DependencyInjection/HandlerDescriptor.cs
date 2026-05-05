using Microsoft.Extensions.DependencyInjection;

namespace ThabeSoft.Mediator.DependencyInjection;


/// <summary>
/// 处理器描述器
/// </summary>
public sealed class HandlerDescriptor : IHandlerDescriptor
{
    private readonly IHandlerDescriptorCollection _root;

    public Type ServiceType { get; }
    public Type ImplementationType { get; }
    public  HandlerKind Kind { get; }
    public  Type MessageType { get; }
    public Type? MessageResponseType { get;  }
    public ServiceLifetime? Lifetime { get; private set; }


    private HandlerDescriptor(IHandlerDescriptorCollection root, Type serviceType, Type implementationType, HandlerKind kind, Type messageType, Type? messageResponseType = null)
    {
        _root = root;
        ServiceType = serviceType;
        ImplementationType = implementationType;
        Kind = kind;
        MessageType = messageType;
        MessageResponseType = messageResponseType;
    }


    public static HandlerDescriptor Command<THandler, TCommand>(IHandlerDescriptorCollection root)
        where THandler : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        return new HandlerDescriptor(root, typeof(ICommandHandler<TCommand>), typeof(THandler), HandlerKind.Command, typeof(TCommand));
    }
    public static HandlerDescriptor Command<THandler, TCommand, TResult>(IHandlerDescriptorCollection root)
        where THandler : ICommandHandler<TCommand, TResult>
        where TCommand : ICommand<TResult>
    {
        return new HandlerDescriptor(root, typeof(ICommandHandler<TCommand, TResult>), typeof(THandler), HandlerKind.CommandWithResult, typeof(TCommand), typeof(TResult));
    }
    public static HandlerDescriptor Query<THandler, TQuery, TResult>(IHandlerDescriptorCollection root)
        where THandler : IQueryHandler<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        return new HandlerDescriptor(root, typeof(IQueryHandler<TQuery, TResult>), typeof(THandler), HandlerKind.Query, typeof(TQuery), typeof(TResult));
    }
    public static HandlerDescriptor Event<THandler, TEvent>(IHandlerDescriptorCollection root)
        where THandler : IEventHandler<TEvent>
        where TEvent : IEvent
    {
        return new HandlerDescriptor(root, typeof(IEventHandler<TEvent>), typeof(THandler), HandlerKind.Event, typeof(TEvent));
    }


    /// <summary>
    /// 设置生命周期
    /// </summary>
    /// <param name="lifetime"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public IHandlerDescriptor SetLifetime(ServiceLifetime lifetime)
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
    public IHandlerDescriptorCollection Except()
    {
        _root.Except(x => x == this);
        return _root;
    }

    /// <summary>
    /// 返回上一级
    /// </summary>
    /// <returns></returns>
    public IHandlerDescriptorCollection Back()
    {
        return _root;
    }


    public static bool operator ==(HandlerDescriptor left, HandlerDescriptor rigt) => Equals(left, rigt);
    public static bool operator !=(HandlerDescriptor left, HandlerDescriptor rigt) => !Equals(left, rigt);


    
    public override bool Equals(object obj)
    {
        if (ReferenceEquals(this, obj)) return true;
        if (obj is not HandlerDescriptor other) return false;

        return Equals(other);
    }
    /// <summary>
    ///  如果 <see cref="ServiceType"/> 相同就认定是一个对象
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public bool Equals(IHandlerDescriptor other)
    {
        return ServiceType == other.ServiceType && ImplementationType == other.ImplementationType;
    }

    /// <summary>
    /// 返回 <see cref="ServiceType"/> 的 HashCode
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        return ServiceType.GetHashCode();
    }
    /// <summary>
    /// HandlerType[Lifetime]
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        if (Lifetime is not null)
        {
            return $"{ServiceType}[{Lifetime}]";
        }

        return $"{ServiceType}";
    }
}