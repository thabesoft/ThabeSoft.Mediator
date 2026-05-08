namespace ThabeSoft.Mediator.SourceGenerator.Models;

/// <summary>
/// 参数类型
/// </summary>
public enum MiddlewareKind : byte
{
    /// <summary>
    /// 泛型
    /// </summary>
    Generic = 1,

    /// <summary>
    /// 具体类型
    /// </summary>
    Concrete = 2,

    /// <summary>
    /// 泛型的具体
    /// </summary>
    GenericConcrete = Generic | Concrete
}