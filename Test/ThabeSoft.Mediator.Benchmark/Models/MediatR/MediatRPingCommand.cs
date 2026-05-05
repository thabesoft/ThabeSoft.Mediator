using MediatR;

namespace ThabeSoft.Mediator.Benchmark.Models.MediatR;

public readonly record struct MediatRPingCommand : IRequest<MediatRPongResponse> { }
public readonly record struct MediatRPongResponse(string Message);



public class MediatRPingCommandHandler : IRequestHandler<MediatRPingCommand, MediatRPongResponse>
{
    public Task<MediatRPongResponse> Handle(MediatRPingCommand request, CancellationToken ct)
    {
        return Task.FromResult(new MediatRPongResponse("Pong"));
    }
}