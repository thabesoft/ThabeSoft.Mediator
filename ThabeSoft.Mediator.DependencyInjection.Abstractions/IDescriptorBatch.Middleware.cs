namespace ThabeSoft.Mediator.DependencyInjection;


/// <summary>
/// 处理器描述批处理
/// </summary>
public interface IMiddlewareDescriptorBatch : 
    IDescriptorBatch<
        IMiddlewareDescriptorBatch,
        IMiddlewareDescriptorCollection>;