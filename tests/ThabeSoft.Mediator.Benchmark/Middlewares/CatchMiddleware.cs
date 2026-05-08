using MediatR;

namespace ThabeSoft.Mediator.Benchmark.Middlewares;


public sealed class CatchMiddleware<TRequest, TResponse> :
    IMiddleware<TRequest, TResponse>,
    MediatR.IPipelineBehavior<TRequest, TResponse>,
    DispatchR.Abstractions.Send.IPipelineBehavior<TRequest, ValueTask<TResponse>>,
    Concordia.IPipelineBehavior<TRequest, TResponse>

    where TRequest : class,
        IRequest<TResponse>,
        MediatR.IRequest<TResponse>,
        DispatchR.Abstractions.Send.IRequest<TRequest, ValueTask<TResponse>>,
        Concordia.IRequest<TResponse>
{
    // ThabeSoft
    ValueTask<TResponse> IMiddleware<TRequest, TResponse>.InvokeAsync(TRequest request, NextMiddleware<TRequest, TResponse> next, CancellationToken cancellationToken)
        => next(request, cancellationToken);

    //MediatR
    Task<TResponse> IPipelineBehavior<TRequest, TResponse>.Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        return next(cancellationToken);
    }

    // DispatchR
    public DispatchR.Abstractions.Send.IRequestHandler<TRequest, ValueTask<TResponse>> NextPipeline { get; set; } = default!;
    ValueTask<TResponse> DispatchR.Abstractions.Send.IRequestHandler<TRequest, ValueTask<TResponse>>.Handle(TRequest request, CancellationToken cancellationToken)
        => NextPipeline.Handle(request, cancellationToken);

    // Concordia
    public Task<TResponse> Handle(TRequest request, Concordia.RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        => next(cancellationToken);
}