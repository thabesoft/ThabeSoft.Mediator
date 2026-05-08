namespace ThabeSoft.Mediator.Benchmark.Messages;


public sealed class PingRequest :
    IRequest<PongResponse>,
    MediatR.IRequest<PongResponse>,
    DispatchR.Abstractions.Send.IRequest<PingRequest, ValueTask<PongResponse>>,
    Concordia.IRequest<PongResponse>;

public record PongResponse(string Message = "Pong");
