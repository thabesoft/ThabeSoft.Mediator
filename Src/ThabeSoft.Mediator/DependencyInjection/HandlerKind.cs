namespace ThabeSoft.Mediator.DependencyInjection;

/// <summary>
/// 处理器类型
/// </summary>
public enum HandlerKind
{
    /// <summary>
    /// 命令
    /// </summary>
    Command,

    /// <summary>
    /// 有结果命令
    /// </summary>
    CommandWithResult,

    /// <summary>
    /// 查询
    /// </summary>
    Query,

    /// <summary>
    /// 事件
    /// </summary>
    Event
}
