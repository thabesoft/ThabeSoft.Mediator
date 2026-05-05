using Microsoft.Extensions.DependencyInjection;

namespace ThabeSoft.Mediator.DependencyInjection;

/// <summary>
/// 处理器描述
/// </summary>
public interface IHandlerDescriptor : IEquatable<IHandlerDescriptor>
{
    /// <summary>
    /// 跟容器
    /// </summary>
    public IHandlerDescriptorCollection Back();

    /// <summary>
    /// 处理器接口类型
    /// </summary>
    public Type ServiceType { get; }
    /// <summary>
    /// 处理器实现类型
    /// </summary>
    public Type ImplementationType { get; }
    /// <summary>
    /// 处理器类型
    /// </summary>
    public HandlerKind Kind { get; }
    /// <summary>
    /// 处理器消息类型
    /// </summary>
    public Type MessageType { get; }
    /// <summary>
    /// 处理器消息响应类型
    /// </summary>
    public Type? MessageResponseType { get; }
    /// <summary>
    /// 处理器生命周期
    /// </summary>
    public ServiceLifetime? Lifetime { get; }


    /// <summary>
    /// 设置生命周期
    /// </summary>
    /// <param name="lifetime"></param>
    /// <returns></returns>
    IHandlerDescriptor SetLifetime(ServiceLifetime lifetime);

    /// <summary>
    /// 从跟容器排除自己
    /// </summary>
    /// <returns></returns>
    IHandlerDescriptorCollection Except();
}


public static class HandlerDescriptorExtensions
{
    extension(IHandlerDescriptor descriptor)
    {
        /// <summary>
        /// 设置为作用域生命周期
        /// </summary>
        /// <returns></returns>
        public IHandlerDescriptor Scoped()
        {
            return descriptor.SetLifetime(ServiceLifetime.Scoped);
        }

        /// <summary>
        /// 设置为单例生命周期
        /// </summary>
        /// <returns></returns>
        public IHandlerDescriptor Singleton()
        {
            return descriptor.SetLifetime(ServiceLifetime.Singleton);
        }

        /// <summary>
        /// 设置为瞬态生命周期
        /// </summary>
        /// <returns></returns>
        public IHandlerDescriptor Transient()
        {
            return descriptor.SetLifetime(ServiceLifetime.Transient);
        }
    }
}