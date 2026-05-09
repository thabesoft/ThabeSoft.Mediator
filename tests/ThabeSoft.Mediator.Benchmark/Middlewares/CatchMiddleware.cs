using MediatR;
using ThabeSoft.Mediator.Benchmark.Messages;

namespace ThabeSoft.Mediator.Benchmark.Middlewares;


public sealed class CatchMiddleware<TRequest, TResponse> :
    IMiddleware<TRequest, TResponse>,
    MediatR.IPipelineBehavior<TRequest, TResponse>,
    DispatchR.Abstractions.Send.IPipelineBehavior<TRequest, ValueTask<TResponse>>

    where TRequest : class,
        IRequest<TResponse>,
        MediatR.IRequest<TResponse>,
        DispatchR.Abstractions.Send.IRequest<TRequest, ValueTask<TResponse>>,
        Concordia.IRequest<TResponse>
{
    // ThabeSoft
    ValueTask<TResponse> IMiddleware<TRequest, TResponse>.InvokeAsync(TRequest request, RequestHandlerDelegateObsolete<TRequest, TResponse> next, CancellationToken cancellationToken)
        => next(request, cancellationToken);

    //MediatR
    Task<TResponse> IRequestPipelineBehavior<TRequest, TResponse>.Handle(TRequest request, MediatR.RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        return next(cancellationToken);
    }

    // DispatchR
    public DispatchR.Abstractions.Send.IRequestHandler<TRequest, ValueTask<TResponse>> NextPipeline { get; set; } = default!;
    ValueTask<TResponse> DispatchR.Abstractions.Send.IRequestHandler<TRequest, ValueTask<TResponse>>.Handle(TRequest request, CancellationToken cancellationToken)
        => NextPipeline.Handle(request, cancellationToken);

    // Concordia
    public Task<PongResponse> Handle(Messages.PingRequest request, Concordia.RequestHandlerDelegate<PongResponse> next, CancellationToken cancellationToken)
        => next(cancellationToken);
}


public sealed class CatchMiddleware : Concordia.IPipelineBehavior<Messages.PingRequest, PongResponse>
{
    // Concordia
    public Task<PongResponse> Handle(Messages.PingRequest request, Concordia.RequestHandlerDelegate<PongResponse> next, CancellationToken cancellationToken)
        => next(cancellationToken);
}