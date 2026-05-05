namespace ThabeSoft.Mediator.Benchmark.Models.ThabeSoft;


public readonly record struct PingCommand : ICommand<PongResponse>;

public readonly record struct PongResponse(string Message);



public class PingCommandHandler : ICommandHandler<PingCommand, PongResponse>
{
    public ValueTask<PongResponse> HandleAsync(PingCommand command, CancellationToken ct)
    {
        return ValueTask.FromResult(new PongResponse("Pong"));
    }
}
