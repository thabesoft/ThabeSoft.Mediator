namespace ThabeSoft.Mediator.DependencyInjection;


/// <summary>
/// 描述批处理
/// </summary>
public interface IDescriptorBatch
{
    /// <summary>
    /// 提交修改并返回根构建器
    /// </summary>
    IDescriptorCollection Apply();

    /// <summary>
    /// 排除所有
    /// </summary>
    /// <returns></returns>
    IDescriptorBatch Except();

    /// <summary>
    /// 批量设置生命周期
    /// </summary>
    /// <param name="lifetime"></param>
    /// <returns></returns>
    IDescriptorBatch SetLifetime(LifetimeKind lifetime);
}


/// <summary>
/// 描述批处理扩展
/// </summary>
public static class DescriptorBatchExtensions
{
    // 硬编码 API
    extension(IDescriptorBatch batch) 
    {
        /// <summary>
        /// 设置为作用域生命周期
        /// </summary>
        /// <returns></returns>
        public IDescriptorBatch Scoped()
        {
            return batch.SetLifetime(LifetimeKind.Scoped);
        }

        /// <summary>
        /// 设置为单例生命周期
        /// </summary>
        /// <returns></returns>
        public IDescriptorBatch Singleton()
        {
            return batch.SetLifetime(LifetimeKind.Singleton);
        }

        /// <summary>
        /// 设置为瞬态生命周期
        /// </summary>
        /// <returns></returns>
        public IDescriptorBatch Transient()
        {
            return batch.SetLifetime(LifetimeKind.Transient);
        }
    }
}
