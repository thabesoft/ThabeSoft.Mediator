using Test.Commands;
using Test.Events;
using Test.Queries;

namespace Test;


[TestClass]
public class MediatorTests : TestBase
{
    [TestMethod]
    public async Task SendAsync_WithCommand_ReturnsResponse()
    {
        // Act
        var response = await Mediator.SendAsync(new PingCommand(), TestContext.CancellationToken);

        // Assert
        Assert.AreEqual("Pong", response.Message);
    }

    [TestMethod]
    public async Task QueryAsync_WithQuery_ReturnsResult()
    {
        var user = await Mediator.QueryAsync(new GetUserQuery(123), TestContext.CancellationToken);

        Assert.AreEqual(123, user.Id);
        Assert.AreEqual("User123", user.Name);
    }

    [TestMethod]
    public async Task PublishAsync_WithEvent_TriggersHandlers()
    {
        await Mediator.PublishAsync(new UserCreatedEvent(1, "张三"), TestContext.CancellationToken);
    }
}