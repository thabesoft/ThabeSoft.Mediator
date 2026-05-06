namespace ThabeSoft.Mediator.Benchmark.Handlers;

public abstract class RequestHandlerBase<TRequest, TResponse> : 
    IRequestHandler<TRequest, TResponse>,
    MediatR.IRequestHandler<TRequest, TResponse>,
    DispatchR.Abstractions.Send.IRequestHandler<TRequest, ValueTask<TResponse>>

    where TRequest : class, IRequest<TResponse>,
        DispatchR.Abstractions.Send.IRequest<TRequest, ValueTask<TResponse>>,
        MediatR.IRequest<TResponse>

{
    protected abstract ValueTask<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken);


    ValueTask<TResponse> DispatchR.Abstractions.Send.IRequestHandler<TRequest, ValueTask<TResponse>>.Handle(TRequest request, CancellationToken cancellationToken)
    {
        return HandleAsync(request, cancellationToken);
    }

    Task<TResponse> MediatR.IRequestHandler<TRequest, TResponse>.Handle(TRequest request, CancellationToken cancellationToken)
    {
        return HandleAsync(request, cancellationToken).AsTask();
    }

    ValueTask<TResponse> IRequestHandler<TRequest, TResponse>.HandleAsync(TRequest request, CancellationToken cancellationToken)
    {
        return HandleAsync(request, cancellationToken);
    }
}


public abstract class RequestHandlerBase<TRequest> :
    IRequestHandler<TRequest>,
    MediatR.IRequestHandler<TRequest>,
    DispatchR.Abstractions.Send.IRequestHandler<TRequest, ValueTask>

    where TRequest : class, IRequest,
        DispatchR.Abstractions.Send.IRequest,
        MediatR.IRequest
{
    protected abstract ValueTask HandleAsync(TRequest request, CancellationToken cancellationToken);

    ValueTask IRequestHandler<TRequest>.HandleAsync(TRequest request, CancellationToken cancellationToken)
    {
        return HandleAsync(request, cancellationToken);
    }
    Task MediatR.IRequestHandler<TRequest>.Handle(TRequest request, CancellationToken cancellationToken)
    {
        return HandleAsync(request, cancellationToken).AsTask();
    }
    ValueTask DispatchR.Abstractions.Send.IRequestHandler<TRequest, ValueTask>.Handle(TRequest request, CancellationToken cancellationToken)
    {
        return HandleAsync(request, cancellationToken);
    }
}