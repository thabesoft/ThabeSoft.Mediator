namespace ThabeSoft.Mediator.Benchmark.Messages;


public sealed class PingRequest : IRequest<PongResponse>,
    MediatR.IRequest<PongResponse>,
    DispatchR.Abstractions.Send.IRequest<PingRequest, ValueTask<PongResponse>>;

public readonly record struct PongResponse(string Message = "Pong");
