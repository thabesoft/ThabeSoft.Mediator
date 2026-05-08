using TRequest = ThabeSoft.Mediator.Benchmark.Messages.PingRequest;
using TResponse = ThabeSoft.Mediator.Benchmark.Messages.PongResponse;


namespace ThabeSoft.Mediator.Benchmark.Handlers;


public sealed class PingPongHandler :
    IRequestHandler<TRequest, TResponse>,
    MediatR.IRequestHandler<TRequest, TResponse>,
    DispatchR.Abstractions.Send.IRequestHandler<TRequest, ValueTask<TResponse>>,
    Concordia.IRequestHandler<TRequest, TResponse>
{
    private static ValueTask<TResponse> ValueHandleAsync(TRequest request, CancellationToken cancellationToken)
    {
        var result = new TResponse($"Pong: {DateTime.Now}");
        return ValueTask.FromResult(result);
    }
    private static Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken)
    {
        var result = new TResponse($"Pong: {DateTime.Now}");
        return Task.FromResult(result);
    }

    // ThabeSoft
    ValueTask<TResponse> IRequestHandler<TRequest, TResponse>.HandleAsync(TRequest request, CancellationToken cancellationToken)
    {
        return ValueHandleAsync(request, cancellationToken);
    }
    // Concordia
    public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
    {
        return HandleAsync(request, cancellationToken);
    }
    // DispatchR
    ValueTask<TResponse> DispatchR.Abstractions.Send.IRequestHandler<TRequest, ValueTask<TResponse>>.Handle(TRequest request, CancellationToken cancellationToken)
    {
        return ValueHandleAsync(request, cancellationToken);
    }
    // MediatR
    Task<TResponse> MediatR.IRequestHandler<TRequest, TResponse>.Handle(TRequest request, CancellationToken cancellationToken)
    {
        return HandleAsync(request, cancellationToken);
    }
}