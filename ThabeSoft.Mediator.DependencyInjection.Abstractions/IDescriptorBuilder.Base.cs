namespace ThabeSoft.Mediator.DependencyInjection;


/// <summary>
/// 描述构建器基类
/// </summary>
/// <typeparam name="TSelf"></typeparam>
/// <typeparam name="TParent"></typeparam>
public abstract class DescriptorBuilderBase<TSelf, TParent, TBatch>(
    TParent root,
    Type serviceType,
    Type implementationType,
    HandlerKind kind,
    Type messageType,
    Type? messageResponseType = null
    ) : 
        IDescriptorBuilder<TSelf, TParent>
    where TParent : DescriptorCollectionBase<TParent, TBatch, TSelf>
    where TSelf : IDescriptorBuilder<TSelf, TParent>

{

    public Type ServiceType { get; } = serviceType;
    public Type ImplementationType { get; } = implementationType;
    public HandlerKind HandlerKind { get; } = kind;
    public Type InputType { get; } = messageType;
    public Type? OutputType { get; } = messageResponseType;
    public LifetimeKind Lifetime { get; private set; }

    /// <summary>
    /// 设置生命周期
    /// </summary>
    /// <param name="lifetime"></param>
    /// <returns></returns>
    public TSelf SetLifetime(LifetimeKind lifetime)
    {
        Lifetime = lifetime;
        return This();
    }

    /// <summary>
    /// 排除自己
    /// </summary>
    /// <returns></returns>
    public TParent Except()
    {
        root.ExceptAll(x => x.Equals(this));
        return root;
    }

    /// <summary>
    /// 返回上一级
    /// </summary>
    /// <returns></returns>
    public TParent Back()
    {
        return root;
    }

    protected abstract TSelf This();



    public override bool Equals(object obj)
    {
        if (ReferenceEquals(this, obj)) return true;
        if (obj is not DescriptorBuilderBase<TSelf, TParent, TBatch> other) return false;

        return Equals(other);
    }

    /// <summary>
    ///  如果 <see cref="ServiceType"/> 和 <see cref="ImplementationType"/> 相同就认定是一个对象
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public bool Equals(DescriptorBuilderBase<TSelf, TParent, TBatch> other)
    {
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