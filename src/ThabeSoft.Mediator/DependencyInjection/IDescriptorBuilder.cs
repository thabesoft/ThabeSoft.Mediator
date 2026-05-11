using Microsoft.Extensions.DependencyInjection;

namespace ThabeSoft.Mediator.DependencyInjection;


/// <summary>
/// 描述构建器
/// </summary>
public interface IDescriptorBuilder
{
    /// <summary>
    /// 种类
    /// </summary>
    DescriptorKind Kind { get; }

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
    ServiceLifetime? Lifetime { get; }

    /// <summary>
    /// 处理器类型
    /// </summary>
    HandlerKind HandlerKind { get; }

    /// <summary>
    /// 设置生命周期
    /// </summary>
    IDescriptorBuilder SetLifetime(ServiceLifetime? lifetime);

    /// <summary>
    /// 跟容器
    /// </summary>
    IDescriptorCollection Back();

    /// <summary>
    /// 从跟容器排除自己
    /// </summary>
    IDescriptorCollection Except();
}


/// <summary>
/// 业务描述构建器扩展
/// </summary>
public static class DescriptorBuilderExtensions
{
    extension(IDescriptorBuilder descriptor)
    {
        /// <summary>
        /// 设置为作用域生命周期
        /// </summary>
        /// <returns></returns>
        public IDescriptorBuilder Scoped()
        {
            return descriptor.SetLifetime(ServiceLifetime.Scoped);
        }

        /// <summary>
        /// 设置为单例生命周期
        /// </summary>
        /// <returns></returns>
        public IDescriptorBuilder Singleton()
        {
            return descriptor.SetLifetime(ServiceLifetime.Singleton);
        }

        /// <summary>
        /// 设置为瞬态生命周期
        /// </summary>
        /// <returns></returns>
        public IDescriptorBuilder Transient()
        {
            return descriptor.SetLifetime(ServiceLifetime.Transient);
        }

        /// <summary>
        /// 不指定生命周期
        /// </summary>
        /// <returns></returns>
        public IDescriptorBuilder None()
        {
            return descriptor.SetLifetime(null);
        }
    }
}
