using Microsoft.Extensions.DependencyInjection;

namespace ThabeSoft.Mediator.DependencyInjection;


/// <summary>
/// 批处理
/// </summary>
/// <param name="root"></param>
/// <param name="matcher"></param>
public class MiddlewareDescriptorBatch(
    IMiddlewareDescriptorCollection root,
    Func<IMiddlewareDescriptor, bool> matcher
    ) : IMiddlewareDescriptorBatch
{
    private readonly Func<IMiddlewareDescriptor, bool> _matcher = matcher;

    public IMiddlewareDescriptorBatch SetLifetime(ServiceLifetime lifetime)
    {
        root.UpdateAll(_matcher, x => x.SetLifetime(lifetime));
        return this;
    }

    public IMiddlewareDescriptorBatch Except()
    {
        root.ExceptAll(_matcher);
        return this;
    }

    public IMiddlewareDescriptorCollection Back()
    {
        return root;
    }
}