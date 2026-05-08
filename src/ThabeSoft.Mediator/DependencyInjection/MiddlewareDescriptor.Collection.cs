using Microsoft.Extensions.DependencyInjection;


namespace ThabeSoft.Mediator.DependencyInjection;


/// <summary>
/// 处理器信息集合
/// </summary>
public sealed class MiddlewareDescriptorCollection(
    ServiceLifetime defaultLifeTime = ServiceLifetime.Scoped
    ) : IMiddlewareDescriptorCollection
{
    // 过滤的
    private readonly List<Func<IMiddlewareDescriptor, bool>> _filters = [];
    // 修改的
    private readonly List<(Func<IMiddlewareDescriptor, bool> Matcher, Action<IMiddlewareDescriptor> Action)> _changes = [];
    // 默认生命周期
    private ServiceLifetime _defaultLifetime = defaultLifeTime;
    // 所有处理器描述
    private readonly List<IMiddlewareDescriptor> _descriptors = [];
    
    /// <summary>
    /// 默认生命周期
    /// </summary>
    public ServiceLifetime DefaultLifetime => _defaultLifetime;


    #region --基础操作--

    /// <summary>
    /// 设置默认处理器生命周期
    /// </summary>
    /// <param name="lifetime"></param>
    /// <returns></returns>
    public IMiddlewareDescriptorCollection SetDefaultLifetime(ServiceLifetime lifetime)
    {
        _defaultLifetime = lifetime;
        return this;
    }
    public IMiddlewareDescriptorCollection UpdateAll(Func<IMiddlewareDescriptor, bool> matcher, Action<IMiddlewareDescriptor> action)
    {
        _changes.Add((matcher, action));
        return this;
    }
    public IMiddlewareDescriptorCollection ExceptAll(Func<IMiddlewareDescriptor, bool> matcher)
    {
        _filters.Add(matcher);
        return this;
    }
    public IMiddlewareDescriptorBatch Batch(Func<IMiddlewareDescriptor, bool> matcher)
    {
        return new MiddlewareDescriptorBatch(this, matcher);
    }

    #endregion



    public IMiddlewareDescriptor AddRequest<TMiddleware, TRequest, TResponse>()
        where TMiddleware : IMiddleware<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        var descriptor = MiddlewareDescriptor.Request<TMiddleware, TRequest, TResponse>(this);
        AddOrUpdateHandlerDescriptor(descriptor);
        return descriptor;
    }
    public IMiddlewareDescriptor AddRequest<TMiddleware, TRequest>()
        where TMiddleware : IMiddleware<TRequest>
        where TRequest : IRequest
    {
        var descriptor = MiddlewareDescriptor.Request<TMiddleware, TRequest>(this);
        AddOrUpdateHandlerDescriptor(descriptor);
        return descriptor;
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
    private void AddOrUpdateHandlerDescriptor(IMiddlewareDescriptor descriptor)
    {
        var index = _descriptors.IndexOf(descriptor);
        if (index != -1)
        {
            _descriptors[index] = descriptor;
        }

        _descriptors.Add(descriptor);
    }
}