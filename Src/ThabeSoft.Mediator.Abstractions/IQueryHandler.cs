namespace ThabeSoft.Mediator;


/// <summary>
/// 查询处理器
/// </summary>
/// <typeparam name="TQuery"></typeparam>
/// <typeparam name="TResult"></typeparam>
public interface IQueryHandler<TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    ValueTask<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}
