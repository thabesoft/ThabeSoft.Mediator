using Microsoft.Extensions.DependencyInjection;
using Microsoft.Testing.Platform.CommandLine;
using ThabeSoft.Mediator;

namespace Test;


[TestClass]
public sealed class CommandTest
{
    [TestMethod]
    public async Task TestMethod1Async()
    {
        ServiceCollection descriptors = new();
        descriptors.AddMediator();

        var services = descriptors.BuildServiceProvider();
        var mediator = services.GetRequiredService<IMediator>();

        var result = await mediator.SendAsync(new Command(), TestContext.CancellationToken);
        Assert.IsNotNull(result);
    }

    public TestContext TestContext { get; set; }
}



public record Command : ICommand<CommandResult>;
public record CommandResult;

public class CommandHandler : ICommandHandler<Command, CommandResult>
{
    public Task<CommandResult> HandleAsync(Command command, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}