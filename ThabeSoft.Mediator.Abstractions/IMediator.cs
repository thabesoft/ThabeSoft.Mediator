using System.Threading;
using System.Threading.Tasks;


namespace ThabeSoft.Mediator
{
    /// <summary>
    /// 中介者
    /// </summary>
    public interface IMediator
    {
        /// <summary>
        /// 命令
        /// </summary>
        /// <param name="command"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task SendAsync(ICommand command, CancellationToken cancellationToken = default);

        /// <summary>
        /// 命令-响应
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="event"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<TResult> SendAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default);

        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<TResult> QueryAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default);

        /// <summary>
        /// 事件
        /// </summary>
        /// <param name="event"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task PublishAsync(IEvent @event, CancellationToken cancellationToken = default);
    }
}