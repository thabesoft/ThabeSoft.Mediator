using MediatR;

namespace Test.Console.Commands;

public class MediatRPingCommandHandler : IRequestHandler<MediatRPingCommand, MediatRPongResponse>
{
    public Task<MediatRPongResponse> Handle(MediatRPingCommand request, CancellationToken ct)
    {
        return Task.FromResult(new MediatRPongResponse("Pong"));
    }
}