namespace ThabeSoft.Mediator.Warppers;


/// <summary>
/// 查询分发器
/// </summary>
public interface IQueryHandlerWarpper : IWarpper
{
    Task<TResult> HandleAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default);
}