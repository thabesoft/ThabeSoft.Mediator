using Microsoft.Extensions.DependencyInjection;

namespace ThabeSoft.Mediator.DependencyInjection;

/// <summary>
/// 处理器描述集合
/// </summary>
public interface IHandlerDescriptorCollection
{
    #region --行为操作--

    /// <summary>
    /// 设置处理器默认生命周期
    /// </summary>
    /// <param name="lifetime"></param>
    /// <returns></returns>
    IHandlerDescriptorCollection SetDefaultLifetime(ServiceLifetime lifetime);

#if DEBUG
    /// <summary>
    /// 查询所有符合条件的处理器并更新
    /// </summary>
    /// <param name="matcher"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    IHandlerDescriptorCollection UpdateAll(Expression<Func<HandlerDescriptor, bool>> matcher, Expression<Action<HandlerDescriptor>> action);

    /// <summary>
    /// 过滤所有符合条件的处理器
    /// </summary>
    /// <param name="matcher"></param>
    /// <returns></returns>
    IHandlerDescriptorCollection Except(Expression<Func<HandlerDescriptor, bool>> matcher);

    /// <summary>
    /// 批处理
    /// </summary>
    /// <param name="matcher"></param>
    /// <returns></returns>
    IHandlerDescriptorBatch FindAll(Expression<Func<HandlerDescriptor, bool>> matcher);
#else
    /// <summary>
    /// 查询所有符合条件的处理器并更新
    /// </summary>
    /// <param name="matcher"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    IHandlerDescriptorCollection UpdateAll(Func<HandlerDescriptor, bool> matcher, Action<HandlerDescriptor> action);

    /// <summary>
    /// 过滤所有符合条件的处理器
    /// </summary>
    /// <param name="matcher"></param>
    /// <returns></returns>
    IHandlerDescriptorCollection Except(Func<HandlerDescriptor, bool> matcher);

    /// <summary>
    /// 批处理
    /// </summary>
    /// <param name="matcher"></param>
    /// <returns></returns>
    IHandlerDescriptorBatch FindAll(Func<HandlerDescriptor, bool> matcher);
#endif

    #endregion

    #region --添加操作--

    /// <summary>
    /// 添加命令
    /// </summary>
    /// <typeparam name="THandler"></typeparam>
    /// <typeparam name="TCommand"></typeparam>
    /// <returns></returns>
    IHandlerDescriptor AddCommand<THandler, TCommand>()
      where THandler : ICommandHandler<TCommand>
      where TCommand : ICommand;

    /// <summary>
    /// 添加命令
    /// </summary>
    /// <typeparam name="THandler"></typeparam>
    /// <typeparam name="TCommand"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    IHandlerDescriptor AddCommand<THandler, TCommand, TResult>()
       where THandler : ICommandHandler<TCommand, TResult>
       where TCommand : ICommand<TResult>;

    /// <summary>
    /// 添加查询
    /// </summary>
    /// <typeparam name="THandler"></typeparam>
    /// <typeparam name="TQeury"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    IHandlerDescriptor AddQuery<THandler, TQeury, TResult>()
       where THandler : IQueryHandler<TQeury, TResult>
       where TQeury : IQuery<TResult>;

    /// <summary>
    /// 添加事件
    /// </summary>
    /// <typeparam name="THandler"></typeparam>
    /// <typeparam name="TEvent"></typeparam>
    /// <returns></returns>
    IHandlerDescriptor AddEvent<THandler, TEvent>()
       where THandler : IEventHandler<TEvent>
       where TEvent : IEvent;

    #endregion
}


/// <summary>
/// 扩展方法
/// </summary>
public static class HandlerDescriptorCollectionExtensions
{
    // 硬编码 API
    extension(IHandlerDescriptorCollection collection)
    {
        public IHandlerDescriptorBatch FindAllByCommand(bool includeResponseCommand = true)
        {
            if (includeResponseCommand)
            {
                return collection.FindAll(x => x.Kind == HandlerKind.Command || x.Kind == HandlerKind.CommandWithResult);
            }
            else
            {
                return collection.FindAll(x => x.Kind == HandlerKind.Command);
            }
        }
        public IHandlerDescriptorBatch FindAllByQuery()
        {
            return collection.FindAll(x => x.Kind == HandlerKind.Query);
        }

        public IHandlerDescriptorBatch FindAllByEvent()
        {
            return collection.FindAll(x => x.Kind == HandlerKind.Event);
        }


        /// <summary>
        /// 根据命令类型查找所有
        /// </summary>
        /// <typeparam name="TCommand"></typeparam>
        /// <returns></returns>
        public IHandlerDescriptorBatch FindAllByCommand<TCommand>()
            where TCommand : ICommand
        {
            var service_type = typeof(ICommandHandler<TCommand>);
            return collection.FindAll(x => x.ServiceType == service_type);
        }
        /// <summary>
        /// 根据命令类型查找所有
        /// </summary>
        /// <typeparam name="TCommand"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public IHandlerDescriptorBatch FindAllByCommand<TCommand, TResult>()
            where TCommand : ICommand<TResult>
        {
            var service_type = typeof(ICommandHandler<TCommand, TResult>);
            return collection.FindAll(x => x.ServiceType == service_type);
        }
        /// <summary>
        /// 根据查询类型查找所有
        /// </summary>
        /// <typeparam name="TQuery"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public IHandlerDescriptorBatch FindAllByQuery<TQuery, TResult>()
            where TQuery : IQuery<TResult>
        {
            var service_type = typeof(IQueryHandler<TQuery, TResult>);
            return collection.FindAll(x => x.ServiceType == service_type);
        }

        /// <summary>
        /// 根据事件类型查找所有
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <returns></returns>
        public IHandlerDescriptorBatch FindAllByEvent<TEvent>()
           where TEvent : IEvent
        {
            var service_type = typeof(IEventHandler<TEvent>);
            return collection.FindAll(x => x.ServiceType == service_type);
        }
    }
}