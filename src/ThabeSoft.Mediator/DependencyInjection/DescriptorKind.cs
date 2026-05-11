namespace ThabeSoft.Mediator.DependencyInjection;


/// <summary>
/// 业务描述种类
/// </summary>
public enum DescriptorKind : byte
{
    /// <summary>
    /// 处理器
    /// </summary>
    Handler,

    /// <summary>
    /// 管道行为
    /// </summary>
    PipelineBehavior,
}