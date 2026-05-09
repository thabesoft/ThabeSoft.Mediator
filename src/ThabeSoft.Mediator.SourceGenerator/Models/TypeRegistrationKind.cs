namespace ThabeSoft.Mediator.SourceGenerator.Models;


/// <summary>
/// 注册种类
/// </summary>
public enum TypeRegistrationKind : byte
{
    /// <summary>
    /// 处理器
    /// </summary>
    Handler = 1,

    /// <summary>
    /// 管道行为
    /// </summary>
    PipelineBehavior = 2,
}