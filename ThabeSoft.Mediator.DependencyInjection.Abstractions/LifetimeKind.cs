using Microsoft.Extensions.DependencyInjection;

namespace ThabeSoft.Mediator.DependencyInjection;


/// <summary>
/// 生命周期类型
/// </summary>
[Flags]
public enum LifetimeKind
{
    /// <summary>
    /// 没有明确指定生命周期类型
    /// </summary>
    None = 0,

    /// <summary>
    /// 单例
    /// </summary>
    Singleton = 1,

    /// <summary>
    /// 作用域
    /// </summary>
    Scoped = 1 << 1,

    /// <summary>
    /// 瞬态
    /// </summary>
    Transient = 1 << 2,

    /// <summary>
    /// 所有
    /// </summary>
    All = Singleton | Scoped | Transient
}



public static class LifetimeKindExtensions
{
    extension(LifetimeKind kind)
    {
        /// <summary>
        /// 转为 <see cref="ServiceLifetime"/> <br/>
        /// 如果是组合<see cref="LifetimeKind"/>
        /// 则按照 <see cref="ServiceLifetime.Singleton"/> > <see cref="ServiceLifetime.Scoped"/> > <see cref="ServiceLifetime.Transient"/> <br/>
        /// 如果不是以上业务类型则使用指定的默认值
        /// </summary>
        /// <param name="defaultLifetime">如果是无法识别的业务则使用此值</param>
        /// <returns></returns>
        public ServiceLifetime ToServiceLifetime(ServiceLifetime defaultLifetime = ServiceLifetime.Scoped)
        {
            if (kind.HasFlag(LifetimeKind.Singleton))
                return ServiceLifetime.Singleton;

            if (kind.HasFlag(LifetimeKind.Scoped))
                return ServiceLifetime.Scoped;

            if (kind.HasFlag(LifetimeKind.Transient))
                return ServiceLifetime.Transient;

            return defaultLifetime;
        }

        /// <summary>
        /// 是否包含此生命周期
        /// </summary>
        public bool HasFlag(ServiceLifetime? serviceLifetime)
        {
            var other = serviceLifetime switch
            {
                ServiceLifetime.Singleton => LifetimeKind.Singleton,
                ServiceLifetime.Scoped => LifetimeKind.Scoped,
                ServiceLifetime.Transient => LifetimeKind.Transient,
                _ => LifetimeKind.None
            };

            return kind.HasFlag(other);
        }
    }
}