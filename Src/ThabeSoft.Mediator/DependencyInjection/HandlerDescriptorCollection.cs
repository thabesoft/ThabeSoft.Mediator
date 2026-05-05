using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Linq.Expressions;

namespace ThabeSoft.Mediator.DependencyInjection;


/// <summary>
/// 处理器信息集合
/// </summary>
public sealed class HandlerDescriptorCollection(ServiceLifetime defaultLifeTime = ServiceLifetime.Scoped) : IHandlerDescriptorCollection
{
#if DEBUG
    // 过滤的
    private readonly List<Expression<Func<HandlerDescriptor, bool>>> _filters = [];
    // 修改的
    private readonly List<(Expression<Func<HandlerDescriptor, bool>> Matcher, Expression<Action<HandlerDescriptor>> Action)> _changes = [];
#else
    // 过滤的
    private readonly List<Func<HandlerDescriptor, bool>> _filters = [];
    // 修改的
    private readonly List<(Func<HandlerDescriptor, bool> Matcher, Action<HandlerDescriptor> Action)> _changes = [];
#endif

    // 默认生命周期
    private ServiceLifetime _defaultLifetime = defaultLifeTime;
    // 所有处理器描述
    private readonly HashSet<HandlerDescriptor> _descriptors = [];


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
#if DEBUG
    /// <summary>
    /// 查询并修改
    /// </summary>
    /// <param name="matcher"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public IHandlerDescriptorCollection UpdateAll(Expression<Func<HandlerDescriptor, bool>> matcher, Expression<Action<HandlerDescriptor>> action)
    {
        _changes.Add((matcher, action));
        return this;
    }
    /// <summary>
    /// 过滤
    /// </summary>
    /// <param name="matcher"></param>
    /// <returns></returns>
    public IHandlerDescriptorCollection Except(Expression<Func<HandlerDescriptor, bool>> matcher)
    {
        _filters.Add(matcher);
        return this;
    }
    /// <summary>
    /// 查询所有符合条件的处理器
    /// </summary>
    /// <param name="matcher"></param>
    /// <returns></returns>
    public IHandlerDescriptorBatch FindAll(Expression<Func<HandlerDescriptor, bool>> matcher)
    {
        return new HandlerDescriptorBatch(this, matcher);
    }
#else
    /// <summary>
    /// 查询并修改
    /// </summary>
    /// <param name="matcher"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public IHandlerDescriptorCollection UpdateAll(Func<HandlerDescriptor, bool> matcher, Action<HandlerDescriptor> action)
    {
        _changes.Add((matcher, action));
        return this;
    }
    /// <summary>
    /// 过滤
    /// </summary>
    /// <param name="matcher"></param>
    /// <returns></returns>
    public IHandlerDescriptorCollection Except(Func<HandlerDescriptor, bool> matcher)
    {
        _filters.Add(matcher);
        return this;
    }
    /// <summary>
    /// 查询所有符合条件的处理器
    /// </summary>
    /// <param name="matcher"></param>
    /// <returns></returns>
    public IHandlerDescriptorBatch FindAll(Func<HandlerDescriptor, bool> matcher)
    {
        return new HandlerDescriptorBatch(this, matcher);
    }
#endif

    #endregion


    /// <summary>
    /// 添加命令
    /// </summary>
    /// <typeparam name="THandler"></typeparam>
    /// <typeparam name="TCommand"></typeparam>
    /// <returns></returns>
    public IHandlerDescriptor AddCommand<THandler, TCommand>()
       where THandler : ICommandHandler<TCommand>
       where TCommand : ICommand
    {
        return GetOrCreateHandlerDescriptor<THandler>(() => HandlerDescriptor.Command<THandler, TCommand>(this));
    }
    /// <summary>
    /// 添加命令
    /// </summary>
    /// <typeparam name="THandler"></typeparam>
    /// <typeparam name="TCommand"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    public IHandlerDescriptor AddCommand<THandler, TCommand, TResult>()
        where THandler : ICommandHandler<TCommand, TResult>
        where TCommand : ICommand<TResult>
    {
        return GetOrCreateHandlerDescriptor<THandler>(() => HandlerDescriptor.Command<THandler, TCommand, TResult>(this));
    }
    /// <summary>
    /// 添加查询
    /// </summary>
    /// <typeparam name="THandler"></typeparam>
    /// <typeparam name="TQeury"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    public IHandlerDescriptor AddQuery<THandler, TQeury, TResult>()
        where THandler : IQueryHandler<TQeury, TResult>
        where TQeury : IQuery<TResult>
    {
        return GetOrCreateHandlerDescriptor<THandler>(() => HandlerDescriptor.Query<THandler, TQeury, TResult>(this));
    }
    /// <summary>
    /// 添加事件
    /// </summary>
    /// <typeparam name="THandler"></typeparam>
    /// <typeparam name="TEvent"></typeparam>
    /// <returns></returns>
    public IHandlerDescriptor AddEvent<THandler, TEvent>()
        where THandler : IEventHandler<TEvent>
        where TEvent : IEvent
    {
        return GetOrCreateHandlerDescriptor<THandler>(() => HandlerDescriptor.Event<THandler, TEvent>(this));
    }

    


    // 构建为服务描述集合
    public IReadOnlyCollection<ServiceDescriptor> BuildToServiceDescriptors()
    {
        var copyd = _descriptors.ToList();

        // 删除
        foreach (var filter in _filters)
        {
#if DEBUG
            var filter_method = filter.Compile();
            Debug.WriteLine(filter.ToString());

            copyd.RemoveAll(x => filter_method(x));
#else
            copyd.RemoveAll(x => filter(x));
#endif
        }
        // 修改
        foreach (var change in _changes)
        {
            for (int i = 0; i < copyd.Count; i++)
            {
                var descriptor = copyd[i];

#if DEBUG
                var change_matcher_method = change.Matcher.Compile();
                var change_action_method = change.Action.Compile();

                Debug.WriteLine($"{change.Matcher}\n{change.Action}");

                if (!change_matcher_method(descriptor)) continue;
                change_action_method(descriptor);
#else
                if (!change.Matcher.Invoke(descriptor)) continue;
                change.Action.Invoke(descriptor);
#endif
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