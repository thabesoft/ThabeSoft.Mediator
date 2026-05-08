using Microsoft.Extensions.DependencyInjection;

namespace ThabeSoft.Mediator.DependencyInjection;


/// <summary>
/// 处理器描述批处理
/// </summary>
public interface IMiddlewareDescriptorBatch
{
    /// <summary>
    /// 返回上一级
    /// </summary>
    IMiddlewareDescriptorCollection Back();

    /// <summary>
    /// 批量排除
    /// </summary>
    /// <returns></returns>
    IMiddlewareDescriptorBatch Except();

    /// <summary>
    /// 批量设置生命周期
    /// </summary>
    /// <param name="lifetime"></param>
    /// <returns></returns>
    IMiddlewareDescriptorBatch SetLifetime(ServiceLifetime lifetime);
}


public static class MiddlewareDescriptorBatchExtensions
{
    // 硬编码 API
    extension(IMiddlewareDescriptorBatch batch)
    {
        /// <summary>
        /// 设置为作用域生命周期
        /// </summary>
        /// <returns></returns>
        public IMiddlewareDescriptorBatch Scoped()
        {
            return batch.SetLifetime(ServiceLifetime.Scoped);
        }

        /// <summary>
        /// 设置为单例生命周期
        /// </summary>
        /// <returns></returns>
        public IMiddlewareDescriptorBatch Singleton()
        {
            return batch.SetLifetime(ServiceLifetime.Singleton);
        }

        /// <summary>
        /// 设置为瞬态生命周期
        /// </summary>
        /// <returns></returns>
        public IMiddlewareDescriptorBatch Transient()
        {
            return batch.SetLifetime(ServiceLifetime.Transient);
        }
    }
}