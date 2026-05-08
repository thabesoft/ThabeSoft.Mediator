using Microsoft.Extensions.DependencyInjection;

namespace ThabeSoft.Mediator.DependencyInjection;


/// <summary>
/// 业务描述集合默认实现
/// </summary>
/// <param name="defaultLifeTime"></param>
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
    /// <param name="lifetime"></param>
    /// <returns></returns>
    public IDescriptorCollection Default(ServiceLifetime lifetime)
    {
        _defaultLifetime = lifetime;
        return this;
    }

    /// <summary>
    /// 更新所有符合条件的元素
    /// </summary>
    /// <param name="matcher"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public IDescriptorCollection UpdateAll(Func<IDescriptorBuilder, bool> matcher, Action<IDescriptorBuilder> action)
    {
        _changes.Add((matcher, action));
        return this;
    }

    /// <summary>
    /// 排除所有符合条件的元素
    /// </summary>
    /// <param name="matcher"></param>
    /// <returns></returns>
    public IDescriptorCollection ExceptAll(Func<IDescriptorBuilder, bool> matcher)
    {
        _filters.Add(matcher);
        return this;
    }

    /// <summary>
    /// 获取批处理结果
    /// </summary>
    /// <param name="matcher">批处理条件</param>
    /// <returns></returns>
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
        return [.. copyd.Select(x => new ServiceDescriptor(x.ServiceType, x.ImplementationType, GetLifetime(x.Lifetime)))];
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




    public IDescriptorBuilder AddRequest<THandler, TRequest, TResponse>()
       where THandler : IRequestHandler<TRequest, TResponse>
       where TRequest : IRequest<TResponse>
    {
        var descriptor = DescriptorBuilder.Request<THandler, TRequest, TResponse>(this);
        AddOrUpdateDescriptor(descriptor);
        return descriptor;
    }
    public IDescriptorBuilder AddRequest<THandler, TRequest>()
        where THandler : IRequestHandler<TRequest>
        where TRequest : IRequest
    {
        var descriptor = DescriptorBuilder.Request<THandler, TRequest>(this);
        AddOrUpdateDescriptor(descriptor);
        return descriptor;
    }
    public IDescriptorBuilder AddRNotification<THandler, TNotification>()
        where THandler : INotificationHandler<TNotification>
        where TNotification : INotification
    {
        var descriptor = DescriptorBuilder.Notification<THandler, TNotification>(this);
        AddOrUpdateDescriptor(descriptor);
        return descriptor;
    }

    public IDescriptorBuilder AddRequestMiddleware<TMiddleware, TRequest, TResponse>()
        where TMiddleware : IMiddleware<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        var descriptor = DescriptorBuilder.RequestMiddlewar<TMiddleware, TRequest, TResponse>(this);
        AddOrUpdateDescriptor(descriptor);
        return descriptor;
    }
    public IDescriptorBuilder AddRequestMiddleware<TMiddleware, TRequest>()
        where TMiddleware : IMiddleware<TRequest>
        where TRequest : IRequest
    {
        var descriptor = DescriptorBuilder.RequestMiddlewar<TMiddleware, TRequest>(this);
        AddOrUpdateDescriptor(descriptor);
        return descriptor;
    }



    /// <summary>
    /// 添加或者更新描述
    /// </summary>
    /// <param name="descriptor"></param>
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