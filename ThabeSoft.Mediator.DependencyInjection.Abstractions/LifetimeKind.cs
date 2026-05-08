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
}
