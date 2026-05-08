namespace ThabeSoft.Mediator.DependencyInjection;

/// <summary>
/// 中间件业务描述
/// </summary>
public interface IMiddlewareDescriptor : 
    IDescriptorBuilder<
        IMiddlewareDescriptor, 
        IMiddlewareDescriptorCollection>;