using Microsoft.Extensions.DependencyInjection;
using System.Linq.Expressions;

namespace ThabeSoft.Mediator.DependencyInjection;


/// <summary>
/// 批处理
/// </summary>
/// <param name="root"></param>
/// <param name="matcher"></param>
#if DEBUG
public class HandlerDescriptorBatch(IHandlerDescriptorCollection root, Expression<Func<HandlerDescriptor, bool>> matcher) : IHandlerDescriptorBatch
{
    private readonly Expression<Func<HandlerDescriptor, bool>> _matcher = matcher;
#else
public class HandlerDescriptorBatch(IHandlerDescriptorCollection root, Func<HandlerDescriptor, bool> matcher) : IHandlerDescriptorBatch
{
    private readonly Func<HandlerDescriptor, bool> _matcher = matcher;
#endif

    public IHandlerDescriptorBatch SetLifetime(ServiceLifetime lifetime)
    {
        root.UpdateAll(_matcher, x => x.SetLifetime(lifetime));
        return this;
    }

    public IHandlerDescriptorBatch Except()
    {
        root.Except(_matcher);
        return this;
    }

    public IHandlerDescriptorCollection Back()
    {
        return root;
    }

#if DEBUG
    public override string ToString()
    {
        return _matcher.ToString();
    }
#endif
}