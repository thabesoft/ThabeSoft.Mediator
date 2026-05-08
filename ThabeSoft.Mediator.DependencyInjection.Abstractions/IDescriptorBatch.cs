namespace ThabeSoft.Mediator.DependencyInjection;


/// <summary>
/// 描述批处理
/// </summary>
/// <typeparam name="TSelft"></typeparam>
/// <typeparam name="TParent"></typeparam>
public interface IDescriptorBatch<TSelft, TParent>
{
    /// <summary>
    /// 提交修改并返回根构建器
    /// </summary>
    TParent Apply();

    /// <summary>
    /// 排除所有
    /// </summary>
    /// <returns></returns>
    TSelft Except();

    /// <summary>
    /// 批量设置生命周期
    /// </summary>
    /// <param name="lifetime"></param>
    /// <returns></returns>
    TSelft SetLifetime(LifetimeKind lifetime);
}


/// <summary>
/// 描述批处理扩展
/// </summary>
public static class DescriptorBatchExtensions
{
    // 硬编码 API
    extension<TSelf, TParent>(IDescriptorBatch<TSelf, TParent> batch) 
        where TSelf : IDescriptorBatch<TSelf, TParent>
    {
        /// <summary>
        /// 设置为作用域生命周期
        /// </summary>
        /// <returns></returns>
        public TSelf Scoped()
        {
            return batch.SetLifetime(LifetimeKind.Scoped);
        }

        /// <summary>
        /// 设置为单例生命周期
        /// </summary>
        /// <returns></returns>
        public TSelf Singleton()
        {
            return batch.SetLifetime(LifetimeKind.Singleton);
        }

        /// <summary>
        /// 设置为瞬态生命周期
        /// </summary>
        /// <returns></returns>
        public TSelf Transient()
        {
            return batch.SetLifetime(LifetimeKind.Transient);
        }
    }
}
