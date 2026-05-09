using Microsoft.Extensions.DependencyInjection;
using ThabeSoft.Mediator.Tests.Messages;

namespace ThabeSoft.Mediator.IntegrationTests;


[TestClass]
public class DependencyInjectionTests
{
    [TestMethod(DisplayName = "注册中介者")]
    public void AddMediator_ShouldRegisterMediator()
    {
        var services = new ServiceCollection();
        services.AddGeneratedMediator();

        var sp = services.BuildServiceProvider();
        Assert.IsNotNull(sp.GetService<IMediator>());
    }

    [TestMethod(DisplayName = "注册处理器")]
    public void AddHandlers_ShouldAutoRegister()
    {
        var services = new ServiceCollection();
        services.AddMediatorHandlers();

        var sp = services.BuildServiceProvider();
        var handler = sp.GetService<IRequestHandler<PingRequest, PongResponse>>();
        Assert.IsNotNull(handler);
    }

    [TestMethod(DisplayName = "注册管道行为")]
    public void AddHandlers_AAAShouldAutoRegister()
    {
        var services = new ServiceCollection();
        services.AddMediatorPipelineBehaviors();

        var sp = services.BuildServiceProvider();
        var handler = sp.GetService<IRequestPipelineBehavior<PingRequest, PongResponse>>();
        Assert.IsNotNull(handler);
    }
}