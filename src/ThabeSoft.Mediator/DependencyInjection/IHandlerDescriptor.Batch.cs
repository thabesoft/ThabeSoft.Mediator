using Microsoft.Extensions.DependencyInjection;

namespace ThabeSoft.Mediator.DependencyInjection;


/// <summary>
/// 处理器描述批处理
/// </summary>
public interface IHandlerDescriptorBatch
{
    /// <summary>
    /// 返回上一级
    /// </summary>
    IHandlerDescriptorCollection Back();

    /// <summary>
    /// 批量排除
    /// </summary>
    /// <returns></returns>
    IHandlerDescriptorBatch Except();

    /// <summary>
    /// 批量设置生命周期
    /// </summary>
    /// <param name="lifetime"></param>
    /// <returns></returns>
    IHandlerDescriptorBatch SetLifetime(ServiceLifetime lifetime);
}


public static class HandlerDescriptorBatchExtensions
{
    // 硬编码 API
    extension(IHandlerDescriptorBatch batch)
    {
        /// <summary>
        /// 设置为作用域生命周期
        /// </summary>
        /// <returns></returns>
        public IHandlerDescriptorBatch Scoped()
        {
            return batch.SetLifetime(ServiceLifetime.Scoped);
        }

        /// <summary>
        /// 设置为单例生命周期
        /// </summary>
        /// <returns></returns>
        public IHandlerDescriptorBatch Singleton()
        {
            return batch.SetLifetime(ServiceLifetime.Singleton);
        }

        /// <summary>
        /// 设置为瞬态生命周期
        /// </summary>
        /// <returns></returns>
        public IHandlerDescriptorBatch Transient()
        {
            return batch.SetLifetime(ServiceLifetime.Transient);
        }
    }
}