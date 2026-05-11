namespace ThabeSoft.Mediator;


/// <summary>
/// 管道业务
/// </summary>
public interface IRequestPipeline<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    ValueTask<TResponse> InvokeAsync(TRequest request, CancellationToken cancellation = default);
}