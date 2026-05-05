using ThabeSoft.Mediator;

namespace Test.Models.Commands;


public record PingCommand : ICommand<PongResponse>;

public record PongResponse(string Message);



// 实现处理器
public class PingCommandHandler : ICommandHandler<PingCommand, PongResponse>
{
    public ValueTask<PongResponse> HandleAsync(PingCommand command, CancellationToken ct)
    {
        return ValueTask.FromResult(new PongResponse("Pong"));
    }
}