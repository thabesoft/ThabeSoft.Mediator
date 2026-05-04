using Microsoft.Extensions.DependencyInjection;
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
        descriptors.AddMediatorHandlers();

        var services = descriptors.BuildServiceProvider();
        var mediator = services.GetRequiredService<IMediator>();

        var result = await mediator.SendAsync(new ResponseCommand(), TestContext.CancellationToken);
        Assert.IsNotNull(result);
    }

    public TestContext TestContext { get; set; }
}