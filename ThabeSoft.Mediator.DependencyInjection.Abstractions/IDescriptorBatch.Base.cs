namespace ThabeSoft.Mediator.DependencyInjection;

public abstract class DescriptorBatchBase<TSelf, TParent, TDescriptor>(TParent root, Func<TDescriptor, bool> matcher) : 
        IDescriptorBatch<TSelf, TParent>
    where TParent : DescriptorCollectionBase<TParent, TSelf, TDescriptor>
    where TDescriptor : IDescriptorBuilder<TDescriptor, TParent>
{
    public TSelf SetLifetime(LifetimeKind lifetime)
    {
        root.UpdateAll(matcher, x => x.SetLifetime(lifetime));
        return This();
    }

    public TSelf Except()
    {
        root.ExceptAll(matcher);
        return This();
    }

    public TParent Apply()
    {
        return root;
    }

    protected abstract TSelf This();
    protected abstract TParent Parent();
}
