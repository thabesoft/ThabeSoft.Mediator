using MediatR;
using ThabeSoft.Mediator.Benchmark.Messages;

namespace ThabeSoft.Mediator.Benchmark.PipelineBehaviors;

public sealed class PipelineBehavior1<TRequest, TResponse> :
    IRequestPipelineBehavior<TRequest, TResponse>,
    MediatR.IPipelineBehavior<TRequest, TResponse>,
    DispatchR.Abstractions.Send.IPipelineBehavior<TRequest, ValueTask<TResponse>>

    where TRequest : class,
        IRequest<TResponse>,
        MediatR.IRequest<TResponse>,
        DispatchR.Abstractions.Send.IRequest<TRequest, ValueTask<TResponse>>
{
    // ThabeSoft
    ValueTask<TResponse> IRequestPipelineBehavior<TRequest, TResponse>.InvokeAsync(TRequest request, HandlerDelegate<TResponse> next, CancellationToken cancellationToken)
       => next(cancellationToken);

    //MediatR
    Task<TResponse> IPipelineBehavior<TRequest, TResponse>.Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        => next(cancellationToken);

    // DispatchR
    public DispatchR.Abstractions.Send.IRequestHandler<TRequest, ValueTask<TResponse>> NextPipeline { get; set; } = default!;
    ValueTask<TResponse> DispatchR.Abstractions.Send.IRequestHandler<TRequest, ValueTask<TResponse>>.Handle(TRequest request, CancellationToken cancellationToken)
        => NextPipeline.Handle(request, cancellationToken);
}



public sealed class ConcordiaPipelineBehavior1 : Concordia.IPipelineBehavior<PingRequest, PongResponse>
{
    // Concordia
    Task<PongResponse> Concordia.IPipelineBehavior<PingRequest, PongResponse>.Handle(PingRequest request, Concordia.RequestHandlerDelegate<PongResponse> next, CancellationToken cancellationToken)
        => next(cancellationToken);
}



public sealed class PipelineBehavior2<TRequest, TResponse> :
    IRequestPipelineBehavior<TRequest, TResponse>,
    MediatR.IPipelineBehavior<TRequest, TResponse>,
    DispatchR.Abstractions.Send.IPipelineBehavior<TRequest, ValueTask<TResponse>>

    where TRequest : class,
        IRequest<TResponse>,
        MediatR.IRequest<TResponse>,
        DispatchR.Abstractions.Send.IRequest<TRequest, ValueTask<TResponse>>
{
    // ThabeSoft
    ValueTask<TResponse> IRequestPipelineBehavior<TRequest, TResponse>.InvokeAsync(TRequest request, HandlerDelegate<TResponse> next, CancellationToken cancellationToken)
       => next(cancellationToken);

    //MediatR
    Task<TResponse> IPipelineBehavior<TRequest, TResponse>.Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        => next(cancellationToken);

    // DispatchR
    public DispatchR.Abstractions.Send.IRequestHandler<TRequest, ValueTask<TResponse>> NextPipeline { get; set; } = default!;
    ValueTask<TResponse> DispatchR.Abstractions.Send.IRequestHandler<TRequest, ValueTask<TResponse>>.Handle(TRequest request, CancellationToken cancellationToken)
        => NextPipeline.Handle(request, cancellationToken);
}
public sealed class ConcordiaPipelineBehavior2 : Concordia.IPipelineBehavior<PingRequest, PongResponse>
{
    // Concordia
    Task<PongResponse> Concordia.IPipelineBehavior<PingRequest, PongResponse>.Handle(PingRequest request, Concordia.RequestHandlerDelegate<PongResponse> next, CancellationToken cancellationToken)
        => next(cancellationToken);
}



public sealed class PipelineBehavior3<TRequest, TResponse> :
    IRequestPipelineBehavior<TRequest, TResponse>,
    MediatR.IPipelineBehavior<TRequest, TResponse>,
    DispatchR.Abstractions.Send.IPipelineBehavior<TRequest, ValueTask<TResponse>>

    where TRequest : class,
        IRequest<TResponse>,
        MediatR.IRequest<TResponse>,
        DispatchR.Abstractions.Send.IRequest<TRequest, ValueTask<TResponse>>
{
    // ThabeSoft
    ValueTask<TResponse> IRequestPipelineBehavior<TRequest, TResponse>.InvokeAsync(TRequest request, HandlerDelegate<TResponse> next, CancellationToken cancellationToken)
       => next(cancellationToken);

    //MediatR
    Task<TResponse> IPipelineBehavior<TRequest, TResponse>.Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        => next(cancellationToken);

    // DispatchR
    public DispatchR.Abstractions.Send.IRequestHandler<TRequest, ValueTask<TResponse>> NextPipeline { get; set; } = default!;
    ValueTask<TResponse> DispatchR.Abstractions.Send.IRequestHandler<TRequest, ValueTask<TResponse>>.Handle(TRequest request, CancellationToken cancellationToken)
        => NextPipeline.Handle(request, cancellationToken);
}
public sealed class ConcordiaPipelineBehavior3 : Concordia.IPipelineBehavior<PingRequest, PongResponse>
{
    // Concordia
    Task<PongResponse> Concordia.IPipelineBehavior<PingRequest, PongResponse>.Handle(PingRequest request, Concordia.RequestHandlerDelegate<PongResponse> next, CancellationToken cancellationToken)
        => next(cancellationToken);
}