using ThabeSoft.Mediator;

namespace Test.Commands;

// 实现处理器
public class PingCommandHandler : ICommandHandler<PingCommand, PongResponse>
{
    public Task<PongResponse> HandleAsync(PingCommand command, CancellationToken ct)
    {
        return Task.FromResult(new PongResponse("Pong"));
    }
}
