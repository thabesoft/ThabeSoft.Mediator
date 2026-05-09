using Microsoft.Extensions.DependencyInjection;

namespace ThabeSoft.Mediator.DependencyInjection;


/// <summary>
/// 描述批量处理器
/// </summary>
/// <param name="root"></param>
/// <param name="matcher"></param>
public sealed class DescriptorBatch(
        DescriptorCollection root,
        Func<IDescriptorBuilder, bool> matcher
    ) :  IDescriptorBatch
{
    public IDescriptorBatch SetLifetime(ServiceLifetime? lifetime)
    {
        root.UpdateAll(matcher, x => x.SetLifetime(lifetime));
        return this;
    }

    public IDescriptorBatch Except()
    {
        root.ExceptAll(matcher);
        return this;
    }

    public IDescriptorCollection Apply()
    {
        return root;
    }
}
