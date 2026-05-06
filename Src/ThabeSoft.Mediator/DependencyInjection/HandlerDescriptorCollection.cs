using Microsoft.Extensions.DependencyInjection;


namespace ThabeSoft.Mediator.DependencyInjection;


/// <summary>
/// 处理器信息集合
/// </summary>
public sealed class HandlerDescriptorCollection(
    ServiceLifetime defaultLifeTime = ServiceLifetime.Scoped
    ) : IHandlerDescriptorCollection
{
    // 过滤的
    private readonly List<Func<HandlerDescriptor, bool>> _filters = [];
    // 修改的
    private readonly List<(Func<HandlerDescriptor, bool> Matcher, Action<HandlerDescriptor> Action)> _changes = [];
    // 默认生命周期
    private ServiceLifetime _defaultLifetime = defaultLifeTime;
    // 所有处理器描述
    private readonly HashSet<HandlerDescriptor> _descriptors = [];
    
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
    public IHandlerDescriptorCollection SetDefaultLifetime(ServiceLifetime lifetime)
    {
        _defaultLifetime = lifetime;
        return this;
    }
    public IHandlerDescriptorCollection UpdateAll(Func<HandlerDescriptor, bool> matcher, Action<HandlerDescriptor> action)
    {
        _changes.Add((matcher, action));
        return this;
    }
    public IHandlerDescriptorCollection ExceptAll(Func<HandlerDescriptor, bool> matcher)
    {
        _filters.Add(matcher);
        return this;
    }
    public IHandlerDescriptorBatch FindAll(Func<HandlerDescriptor, bool> matcher)
    {
        return new HandlerDescriptorBatch(this, matcher);
    }

    #endregion


    public IHandlerDescriptor AddRequest<THandler, TCommand>()
       where THandler : IRequestHandler<TCommand>
       where TCommand : IRequest
    {
        return GetOrCreateHandlerDescriptor<THandler>(() => HandlerDescriptor.Request<THandler, TCommand>(this));
    }

    public IHandlerDescriptor AddRequest<THandler, TCommand, TResult>()
        where THandler : IRequestHandler<TCommand, TResult>
        where TCommand : IRequest<TResult>
    {
        return GetOrCreateHandlerDescriptor<THandler>(() => HandlerDescriptor.Request<THandler, TCommand, TResult>(this));
    }

    public IHandlerDescriptor AddNotification<THandler, TEvent>()
        where THandler : INotificationHandler<TEvent>
        where TEvent : INotification
    {
        return GetOrCreateHandlerDescriptor<THandler>(() => HandlerDescriptor.Notification<THandler, TEvent>(this));
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
    private HandlerDescriptor GetOrCreateHandlerDescriptor<THandler>(Func<HandlerDescriptor> factory)
    {
        var handler_type = typeof(THandler);
        var descriptor = _descriptors.FirstOrDefault(x => x.ImplementationType == handler_type);
        if (descriptor is not null) return descriptor;

        descriptor = factory.Invoke();
        _descriptors.Add(descriptor);
        return descriptor;
    }
}