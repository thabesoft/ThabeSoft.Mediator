using Microsoft.Extensions.DependencyInjection;

namespace ThabeSoft.Mediator.DependencyInjection;


/// <summary>
/// 业务描述集合默认实现
/// </summary>
/// <typeparam name="TSelf"></typeparam>
/// <typeparam name="TBatch"></typeparam>
/// <typeparam name="TDescriptor"></typeparam>
/// <param name="defaultLifeTime"></param>
public abstract class DescriptorCollectionBase<TSelf, TBatch, TDescriptor>(ServiceLifetime defaultLifeTime) : 
        IDescriptorCollection<TSelf, TBatch, TDescriptor>
    where TDescriptor : IDescriptorBuilder<TDescriptor, TSelf>
{
    // 过滤的
    private readonly List<Func<TDescriptor, bool>> _filters = [];
    // 修改的
    private readonly List<(Func<TDescriptor, bool> Matcher, Action<TDescriptor> Action)> _changes = [];
    // 默认生命周期
    private ServiceLifetime _defaultLifetime = defaultLifeTime;
    // 所有处理器描述
    private readonly List<TDescriptor> _descriptors = [];

    /// <summary>
    /// 默认生命周期
    /// </summary>
    public ServiceLifetime DefaultLifetime => _defaultLifetime;


    /// <summary>
    /// 设置默认处理器生命周期
    /// </summary>
    /// <param name="lifetime"></param>
    /// <returns></returns>
    public TSelf Default(ServiceLifetime lifetime)
    {
        _defaultLifetime = lifetime;
        return This();
    }

    /// <summary>
    /// 更新所有符合条件的元素
    /// </summary>
    /// <param name="matcher"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public TSelf UpdateAll(Func<TDescriptor, bool> matcher, Action<TDescriptor> action)
    {
        _changes.Add((matcher, action));
        return This();
    }

    /// <summary>
    /// 排除所有符合条件的元素
    /// </summary>
    /// <param name="matcher"></param>
    /// <returns></returns>
    public TSelf ExceptAll(Func<TDescriptor, bool> matcher)
    {
        _filters.Add(matcher);
        return This();
    }


    // 构建为服务描述集合
    public IReadOnlyCollection<ServiceDescriptor> BuildToServiceDescriptors()
    {
        var copyd = _descriptors.ToList();

        // 删除
        foreach (var filter in _filters)
        {
            copyd.RemoveAll(x => filter(x));
        }
        // 修改
        foreach (var change in _changes)
        {
            for (int i = 0; i < copyd.Count; i++)
            {
                var descriptor = copyd[i];
                if (!change.Matcher.Invoke(descriptor)) continue;
                change.Action.Invoke(descriptor);
            }
        }

        // 构建
        return [.. copyd.Select(x => new ServiceDescriptor(x.ServiceType, x.ImplementationType, GetLifetime(x.Lifetime)))];
    }

   
    /// <summary>
    /// 返回自己
    /// </summary>
    /// <returns></returns>
    protected abstract TSelf This();

    /// <summary>
    /// 获取批处理结果
    /// </summary>
    /// <param name="matcher">批处理条件</param>
    /// <returns></returns>
    public abstract TBatch Batch(Func<TDescriptor, bool> matcher);

    /// <summary>
    /// 添加或者更新描述
    /// </summary>
    /// <param name="descriptor"></param>
    protected void AddOrUpdateDescriptor(TDescriptor descriptor)
    {
        var index = _descriptors.IndexOf(descriptor);
        if (index != -1)
        {
            _descriptors[index] = descriptor;
        }

        _descriptors.Add(descriptor);
    }


    // 转为微软的生命周期
    private ServiceLifetime GetLifetime(LifetimeKind lifetime)
    {
        return lifetime switch
        {
            LifetimeKind.Singleton => ServiceLifetime.Singleton,
            LifetimeKind.Scoped => ServiceLifetime.Scoped,
            LifetimeKind.Transient => ServiceLifetime.Transient,
            _ => _defaultLifetime
        };
    }
}