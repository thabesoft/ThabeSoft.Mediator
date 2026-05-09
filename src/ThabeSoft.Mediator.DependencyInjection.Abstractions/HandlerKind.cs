namespace ThabeSoft.Mediator.DependencyInjection;


/// <summary>
/// 处理器类型
/// </summary>
public enum HandlerKind
{
    /// <summary>
    /// 请求
    /// </summary>
    Request,

    /// <summary>
    /// 请求-响应
    /// </summary>
    RequestResponse,

    /// <summary>
    /// 通知
    /// </summary>
    Notification
}
