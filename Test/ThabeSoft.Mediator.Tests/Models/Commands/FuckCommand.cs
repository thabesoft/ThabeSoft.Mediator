using ThabeSoft.Mediator;

namespace Test.Models.Commands;

public record FuckCommand : ICommand;

public class FuckCommandHandler : ICommandHandler<FuckCommand>
{
    public ValueTask HandleAsync(FuckCommand command, CancellationToken ct)
    {
        return default;
    }
}
