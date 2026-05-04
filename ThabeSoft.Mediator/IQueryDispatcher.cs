using System.Threading;
using System.Threading.Tasks;

namespace ThabeSoft.Mediator
{
    /// <summary>
    /// 查询分发器
    /// </summary>
    public interface IQueryDispatcher
    {
        Task<TResult> DispatchAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default);
    }
}