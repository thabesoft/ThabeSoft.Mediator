using Microsoft.Extensions.DependencyInjection;

namespace ThabeSoft.Mediator.DependencyInjection;


/// <summary>
/// 业务描述集合默认实现
/// </summary>
public sealed class DescriptorCollection(ServiceLifetime defaultLifeTime = ServiceLifetime.Scoped) : IDescriptorCollection
{
    // 过滤的
    private readonly List<Func<IDescriptorBuilder, bool>> _filters = [];
    // 修改的
    private readonly List<(Func<IDescriptorBuilder, bool> Matcher, Action<IDescriptorBuilder> Action)> _changes = [];
    // 默认生命周期
    private ServiceLifetime _defaultLifetime = defaultLifeTime;
    // 所有处理器描述
    private readonly List<IDescriptorBuilder> _descriptors = [];


    /// <summary>
    /// 默认生命周期
    /// </summary>
    public ServiceLifetime DefaultLifetime => _defaultLifetime;


    /// <summary>
    /// 设置默认处理器生命周期
    /// </summary>
    public IDescriptorCollection Default(ServiceLifetime lifetime)
    {
        _defaultLifetime = lifetime;
        return this;
    }

    /// <summary>
    /// 更新所有符合条件的元素
    /// </summary>
    public IDescriptorCollection UpdateAll(Func<IDescriptorBuilder, bool> matcher, Action<IDescriptorBuilder> action)
    {
        _changes.Add((matcher, action));
        return this;
    }

    /// <summary>
    /// 排除所有符合条件的元素
    /// </summary>
    public IDescriptorCollection ExceptAll(Func<IDescriptorBuilder, bool> matcher)
    {
        _filters.Add(matcher);
        return this;
    }

    /// <summary>
    /// 获取批处理结果
    /// </summary>
    /// <param name="matcher">批处理条件</param>
    public IDescriptorBatch Batch(Func<IDescriptorBuilder, bool> matcher)
    {
        return new DescriptorBatch(this, matcher);
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
        return [.. copyd.Select(x => new ServiceDescriptor(x.ServiceType, x.ImplementationType, x.Lifetime ?? _defaultLifetime))];
    }

    public void SyncToServiceCollection(IServiceCollection descriptors)
    {
        var works = _descriptors.ToList();

        // 需要删除的
        var removing_builders = works.FindAll(x => _filters.Any(f => f.Invoke(x))).ToArray();
        // 删除业务
        foreach (var item in removing_builders.SelectMany(x => FindAll(x))) descriptors.Remove(item);
        foreach (var item in removing_builders) works.Remove(item);

        // 需要改变的业务
        var change_builders = _changes.SelectMany(change => works.Where(x => change.Matcher(x))).ToArray();
        var changes = change_builders.SelectMany(builder => FindAll(builder).Select(removed => (builder, removed))).ToArray();

        foreach (var (builder, removed) in changes)
        {
            descriptors.Remove(removed);
            descriptors.Add(ToServiceDescriptor(builder));
            works.Remove(builder);
        }

        // 新增的
        var adds = works.Where(x => !FindAll(x).Any());
        foreach (var item in adds.ToArray())
        {
            descriptors.Add(ToServiceDescriptor(item));
            works.Remove(item);
        }

        // TODO: 没处理完
        if (works.Count != 0) return;


        // 转为 ServiceDescriptor
        ServiceDescriptor ToServiceDescriptor(IDescriptorBuilder builder)
        {
            return new ServiceDescriptor(builder.ServiceType, builder.ImplementationType, builder.Lifetime ?? _defaultLifetime);
        }
        // 根据业务类型和实现类型查询
        IEnumerable<ServiceDescriptor> FindAll(IDescriptorBuilder builder)
        {
            for (int i = 0; i < descriptors.Count; i++)
            {
                var service = descriptors[i];

                if (service.ServiceType != builder.ServiceType) continue;
                if (service.ImplementationType != builder.ImplementationType) continue;

                yield return service;
            }
        }
    }


    /// <summary>
    /// 添加请求处理器
    /// </summary>
    public IDescriptorBuilder AddRequestHandler<THandler, TRequest>()
       where THandler : IRequestHandler<TRequest>
       where TRequest : IRequest
    {
        var descriptor = DescriptorBuilder.RequestHandler<THandler, TRequest>(this);
        AddOrUpdateDescriptor(descriptor);
        return descriptor;
    }
    /// <summary>
    /// 添加请求处理器
    /// </summary>
    public IDescriptorBuilder AddRequestHandler<THandler, TRequest, TResponse>()
       where THandler : IRequestHandler<TRequest, TResponse>
       where TRequest : IRequest<TResponse>
    {
        var descriptor = DescriptorBuilder.RequestHandler<THandler, TRequest, TResponse>(this);
        AddOrUpdateDescriptor(descriptor);
        return descriptor;
    }
    /// <summary>
    /// 添加通知处理器
    /// </summary>
    public IDescriptorBuilder AddNotificationHandler<THandler, TNotification>()
        where THandler : INotificationHandler<TNotification>
        where TNotification : INotification
    {
        var descriptor = DescriptorBuilder.NotificationHandler<THandler, TNotification>(this);
        AddOrUpdateDescriptor(descriptor);
        return descriptor;
    }

    /// <summary>
    /// 添加请求管道行为
    /// </summary>
    public IDescriptorBuilder AddRequestBehavior<TBehavior, TRequest>()
        where TBehavior : IRequestPipelineBehavior<TRequest>
        where TRequest : IRequest
    {
        var descriptor = DescriptorBuilder.RequestBehavior<TBehavior, TRequest>(this);
        AddOrUpdateDescriptor(descriptor);
        return descriptor;
    }
    /// <summary>
    /// 添加请求管道行为
    /// </summary>
    public IDescriptorBuilder AddRequestBehavior<TBehavior, TRequest, TResponse>()
        where TBehavior : IRequestPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        var descriptor = DescriptorBuilder.RequestBehavior<TBehavior, TRequest, TResponse>(this);
        AddOrUpdateDescriptor(descriptor);
        return descriptor;
    }
    /// <summary>
    /// 添加通知管道行为
    /// </summary>
    public IDescriptorBuilder AddNotificationBehavior<TBehavior, TNotification>()
       where TBehavior : INotificationPipelineBehavior<TNotification>
       where TNotification : INotification
    {
        var descriptor = DescriptorBuilder.NotificationBehavior<TBehavior, TNotification>(this);
        AddOrUpdateDescriptor(descriptor);
        return descriptor;
    }



    /// <summary>
    /// 添加或者更新描述
    /// </summary>
    private void AddOrUpdateDescriptor(IDescriptorBuilder descriptor)
    {
        var index = _descriptors.IndexOf(descriptor);
        if (index != -1)
        {
            _descriptors[index] = descriptor;
        }

        _descriptors.Add(descriptor);
    }
}