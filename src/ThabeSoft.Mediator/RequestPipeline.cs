using Microsoft.Extensions.DependencyInjection;
using System.Runtime.InteropServices;

namespace ThabeSoft.Mediator;


/// <summary>
/// 请求管道
/// </summary>
[Obsolete]
internal class RequestPipeline<TRequest>(
        TRequest request,
        IRequestHandler<TRequest> handler,
        IRequestPipelineBehavior<TRequest>[] behaviors
    ) where TRequest : IRequest
{
    public ValueTask InvokeAsync(CancellationToken cancellationToken)
    {
        return MoveNext(cancellationToken);
    }

    private ValueTask MoveNext(CancellationToken cancellationToken)
    {
        HandlerDelegate next = (ct) => handler.HandleAsync(request, ct);
        for (int i = behaviors.Length - 1; i >= 0; i--)
        {
            var behavior = behaviors[i];
            var currentNext = next;
            next = (c) => behavior.InvokeAsync(request, currentNext, c);  // 只分配一次
        }

        return next(cancellationToken);


        //if (_index >= behaviors.Length) return handler.HandleAsync(request, cancellationToken);

        //var behavior = behaviors[_index++];
        //return behavior.InvokeAsync(request, MoveNext, cancellationToken);
    }
}

/// <summary>
/// 请求管道
/// </summary>
[Obsolete]
internal class RequestPipeline<TRequest, TResponse>(
        TRequest request,
        IRequestHandler<TRequest, TResponse> handler,
        IRequestPipelineBehavior<TRequest, TResponse>[] behaviors
    ) where TRequest : IRequest<TResponse>
{
    public ValueTask<TResponse> InvokeAsync(CancellationToken cancellationToken)
    {
        return MoveNext(cancellationToken);
    }

    private ValueTask<TResponse> MoveNext(CancellationToken cancellationToken)
    {
        HandlerDelegate<TResponse> next = (ct) => handler.HandleAsync(request, ct);
        for (int i = behaviors.Length - 1; i >= 0; i--)
        {
            var behavior = behaviors[i];
            var currentNext = next;
            next = (c) => behavior.InvokeAsync(request, currentNext, c);  // 只分配一次
        }

        return next(cancellationToken);


        //if (_index >= behaviors.Length) return handler.HandleAsync(request, cancellationToken);

        //var behavior = behaviors[_index++];
        //return behavior.InvokeAsync(request, MoveNext, cancellationToken);
    }
}