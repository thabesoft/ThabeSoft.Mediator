namespace ThabeSoft.Mediator.DependencyInjection;


/// <summary>
/// 有处理器种类的
/// </summary>
public interface IHandlerKindable
{
    /// <summary>
    /// 处理器类型
    /// </summary>
    HandlerKind HandlerKind { get; }
}