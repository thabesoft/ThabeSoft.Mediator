using Microsoft.Extensions.DependencyInjection;

namespace ThabeSoft.Mediator.UnitTests;


[TestClass]
public class DependencyInjectionTests
{
    [TestMethod(DisplayName = "中介者注册")]
    public void AddGeneratedMediator()
    {
        var services = new ServiceCollection();
        services.AddMediator(ServiceLifetime.Singleton);

        var sp = services.BuildServiceProvider();
        Assert.IsNotNull(sp.GetService<IMediator>());
        Assert.IsNotNull(sp.GetService<ISender>());
        Assert.IsNotNull(sp.GetService<IPublisher>());
    }


    [TestMethod(DisplayName = "中介者重复注册")]
    public void AddMediator()
    {
        var services = new ServiceCollection();

        services.AddMediator(ServiceLifetime.Singleton);
        Assert.HasCount(3, services);

        services.AddMediator(ServiceLifetime.Scoped);
        Assert.HasCount(3, services);

        services.AddMediator(ServiceLifetime.Transient);
        Assert.HasCount(3, services);
    }
}