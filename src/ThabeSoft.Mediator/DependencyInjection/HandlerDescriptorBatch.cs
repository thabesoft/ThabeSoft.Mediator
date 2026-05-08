using Microsoft.Extensions.DependencyInjection;

namespace ThabeSoft.Mediator.DependencyInjection;


/// <summary>
/// 批处理
/// </summary>
/// <param name="root"></param>
/// <param name="matcher"></param>
public class HandlerDescriptorBatch(
    HandlerDescriptorCollection root,
    Func<HandlerDescriptor, bool> matcher
    ) : DescriptorBatchBase<HandlerDescriptor, HandlerDescriptorCollection, HandlerDescriptor>
{
    private readonly Func<HandlerDescriptor, bool> _matcher = matcher;

    public IHandlerDescriptorBatch SetLifetime(ServiceLifetime lifetime)
    {
        root.UpdateAll(_matcher, x => x.SetLifetime(lifetime));
        return this;
    }

    public IHandlerDescriptorBatch Except()
    {
        root.ExceptAll(_matcher);
        return this;
    }

    public IHandlerDescriptorCollection Apply()
    {
        return root;
    }
}