using System.Threading;
using System.Threading.Tasks;


namespace ThabeSoft.Mediator
{
    /// <summary>
    /// 事件分发器
    /// </summary>
    public interface IEventDispatcher
    {
        Task DispatchAsync(IEvent @event, CancellationToken cancellationToken = default);
    }
}