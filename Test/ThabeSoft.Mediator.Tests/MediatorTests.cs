using Microsoft.Extensions.DependencyInjection;
using Test.Models.Commands;
using Test.Models.Events;
using Test.Models.Queries;

namespace ThabeSoft.Mediator.Tests;


/// <summary>
/// 中介者测试
/// </summary>
[TestClass]
public class MediatorTests : MediatorTestBase
{

    [TestMethod(DisplayName = "请求结果是否一致")]
    public async Task SendAsync_WithCommand_ReturnsResponse()
    {
        // Act
        var response = await Mediator.SendAsync(new PingCommand(), TestContext.CancellationToken);

        // Assert
        Assert.AreEqual("Pong", response.Message);
    }

    [TestMethod(DisplayName = "查询结果是否一致")]
    public async Task QueryAsync_WithQuery_ReturnsResult()
    {
        var user = await Mediator.QueryAsync(new GetUserQuery(123), TestContext.CancellationToken);

        Assert.AreEqual(123, user.Id);
        Assert.AreEqual("User123", user.Name);
    }

    [TestMethod(DisplayName = "发布事件")]
    public async Task PublishAsync_WithEvent_TriggersHandlers()
    {
        await Mediator.PublishAsync(new UserCreatedEvent(1, "张三"), TestContext.CancellationToken);
    }

    [TestMethod]
    public async Task ExceptionTest_NotFoundCommand_ThrowsNotSupported()
    {
        // 未注册的命令
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(async () =>
        {
            await Mediator.SendAsync(new UnregisteredCommand(), TestContext.CancellationToken);
        });
    }

}