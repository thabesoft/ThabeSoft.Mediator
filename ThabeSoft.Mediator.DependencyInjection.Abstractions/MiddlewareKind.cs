namespace ThabeSoft.Mediator.DependencyInjection;


/// <summary>
/// 中间件类型
/// </summary>
public enum MiddlewareKind : byte
{
    /// <summary>
    /// 开放类型
    /// </summary>
    Request,

    /// <summary>
    /// 封闭类型
    /// </summary>
    RequestResponse,
}