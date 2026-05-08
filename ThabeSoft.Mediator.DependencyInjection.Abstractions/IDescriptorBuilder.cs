namespace ThabeSoft.Mediator.DependencyInjection;


/// <summary>
/// 描述构建器
/// </summary>
/// <typeparam name="TParent"></typeparam>
public interface IDescriptorBuilder<TSelf, TParent> :  ILifetimeBuilder
{
    /// <summary>
    /// 业务类型
    /// </summary>
    Type ServiceType { get; }

    /// <summary>
    /// 实现类型
    /// </summary>
    Type ImplementationType { get; }

    /// <summary>
    /// 输入类型
    /// </summary>
    Type InputType { get; }

    /// <summary>
    /// 输出类型
    /// </summary>
    Type? OutputType { get; }

    /// <summary>
    /// 处理器生命周期
    /// </summary>
    LifetimeKind Lifetime { get; }

    /// <summary>
    /// 处理器类型
    /// </summary>
    HandlerKind HandlerKind { get; }

    /// <summary>
    /// 设置生命周期
    /// </summary>
    /// <param name="lifetime"></param>
    /// <returns></returns>
    TSelf SetLifetime(LifetimeKind lifetime);

    /// <summary>
    /// 跟容器
    /// </summary>
    TParent Back();

    /// <summary>
    /// 从跟容器排除自己
    /// </summary>
    /// <returns></returns>
    TParent Except();
}


/// <summary>
/// 业务描述构建器扩展
/// </summary>
public static class DescriptorBuilderExtensions
{
    extension<T, Tparent>(IDescriptorBuilder<T, Tparent> descriptor)
    {
        /// <summary>
        /// 设置为作用域生命周期
        /// </summary>
        /// <returns></returns>
        public T Scoped()
        {
            return descriptor.SetLifetime(LifetimeKind.Scoped);
        }

        /// <summary>
        /// 设置为单例生命周期
        /// </summary>
        /// <returns></returns>
        public T Singleton()
        {
            return descriptor.SetLifetime(LifetimeKind.Singleton);
        }

        /// <summary>
        /// 设置为瞬态生命周期
        /// </summary>
        /// <returns></returns>
        public T Transient()
        {
            return descriptor.SetLifetime(LifetimeKind.Transient);
        }

        /// <summary>
        /// 不指定生命周期
        /// </summary>
        /// <returns></returns>
        public T None()
        {
            return descriptor.SetLifetime(LifetimeKind.None);
        }
    }
}
