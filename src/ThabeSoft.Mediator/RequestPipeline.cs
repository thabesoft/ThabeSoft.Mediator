namespace ThabeSoft.Mediator;


/// <summary>
/// 请求管道
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <param name="request"></param>
/// <param name="handler"></param>
/// <param name="behaviors"></param>
internal struct RequestPipeline<TRequest>(
        TRequest request,
        IRequestHandler<TRequest> handler,
        IRequestPipelineBehavior<TRequest>[] behaviors
    ) where TRequest : IRequest
{
    // 当前执行行为索引
    private int _index;

    public ValueTask InvokeAsync(CancellationToken cancellationToken)
    {
        return MoveNext(cancellationToken);
    }

    private ValueTask MoveNext(CancellationToken cancellationToken)
    {
        if (_index >= behaviors.Length) return handler.HandleAsync(request, cancellationToken);

        var behavior = behaviors[_index++];
        return behavior.InvokeAsync(request, MoveNext, cancellationToken);
    }
}

/// <summary>
/// 请求管道
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
/// <param name="request"></param>
/// <param name="handler"></param>
/// <param name="behaviors"></param>
internal struct RequestPipeline<TRequest, TResponse>(
        TRequest request,
        IRequestHandler<TRequest, TResponse> handler,
        IRequestPipelineBehavior<TRequest, TResponse>[] behaviors
    ) where TRequest : IRequest<TResponse>
{
    // 当前执行行为索引
    private int _index;

    public ValueTask<TResponse> InvokeAsync(CancellationToken cancellationToken)
    {
        return MoveNext(cancellationToken);
    }

    private ValueTask<TResponse> MoveNext(CancellationToken cancellationToken)
    {
        if (_index >= behaviors.Length) return handler.HandleAsync(request, cancellationToken);

        var behavior = behaviors[_index++];
        return behavior.InvokeAsync(request, MoveNext, cancellationToken);
    }
}