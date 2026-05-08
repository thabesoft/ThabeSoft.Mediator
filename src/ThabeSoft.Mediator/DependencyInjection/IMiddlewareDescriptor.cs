using Microsoft.Extensions.DependencyInjection;

namespace ThabeSoft.Mediator.DependencyInjection;

/// <summary>
/// 处理器描述
/// </summary>
public interface IMiddlewareDescriptor : IEquatable<IMiddlewareDescriptor>
{
    /// <summary>
    /// 跟容器
    /// </summary>
    public IMiddlewareDescriptorCollection Back();

    /// <summary>
    /// 接口类型
    /// </summary>
    public Type ServiceType { get; }
    /// <summary>
    /// 实现类型
    /// </summary>
    public Type ImplementationType { get; }
    /// <summary>
    /// 处理器类型
    /// </summary>
    public MiddlewareKind Kind { get; }
    /// <summary>
    /// 结果类型
    /// </summary>
    public Type? OutputType { get; }
    /// <summary>
    /// 生命周期
    /// </summary>
    public ServiceLifetime? Lifetime { get; }


    /// <summary>
    /// 设置生命周期
    /// </summary>
    /// <param name="lifetime"></param>
    /// <returns></returns>
    IMiddlewareDescriptor SetLifetime(ServiceLifetime lifetime);

    /// <summary>
    /// 从跟容器排除自己
    /// </summary>
    /// <returns></returns>
    IMiddlewareDescriptorCollection Except();
}


public static class MiddlewareDescriptorExtensions
{
    extension(IMiddlewareDescriptor descriptor)
    {
        /// <summary>
        /// 设置为作用域生命周期
        /// </summary>
        /// <returns></returns>
        public IMiddlewareDescriptor Scoped()
        {
            return descriptor.SetLifetime(ServiceLifetime.Scoped);
        }

        /// <summary>
        /// 设置为单例生命周期
        /// </summary>
        /// <returns></returns>
        public IMiddlewareDescriptor Singleton()
        {
            return descriptor.SetLifetime(ServiceLifetime.Singleton);
        }

        /// <summary>
        /// 设置为瞬态生命周期
        /// </summary>
        /// <returns></returns>
        public IMiddlewareDescriptor Transient()
        {
            return descriptor.SetLifetime(ServiceLifetime.Transient);
        }
    }
}