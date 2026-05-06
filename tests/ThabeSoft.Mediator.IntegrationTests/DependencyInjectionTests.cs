using Microsoft.Extensions.DependencyInjection;
using ThabeSoft.Mediator.Tests.Messages;

namespace ThabeSoft.Mediator.IntegrationTests;


[TestClass]
public class DependencyInjectionTests
{
    [TestMethod(DisplayName = "注册到Ioc容器")]
    public void AddMediator_ShouldRegisterMediator()
    {
        var services = new ServiceCollection();
        services.AddMediator();

        var sp = services.BuildServiceProvider();
        Assert.IsNotNull(sp.GetService<IMediator>());
    }

    [TestMethod(DisplayName = "自动处理器到Ioc容器")]
    public void AddHandlers_ShouldAutoRegister()
    {
        var services = new ServiceCollection();
        services.AddMediatorHandlers();

        var sp = services.BuildServiceProvider();
        var handler = sp.GetService<IRequestHandler<PingRequest, PongResponse>>();
        Assert.IsNotNull(handler);
    }
}