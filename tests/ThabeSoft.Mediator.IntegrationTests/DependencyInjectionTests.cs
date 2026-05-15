using Microsoft.Extensions.DependencyInjection;

namespace ThabeSoft.Mediator.IntegrationTests;


[TestClass]
public class DependencyInjectionTests
{
    [TestMethod(DisplayName = "注册生成代码")]
    public void AddGeneratedMediator()
    {
        var services = new ServiceCollection();
        services.AddMediator();
        services.ConfigureMediator(x => x.Default(ServiceLifetime.Singleton));

        var sp = services.BuildServiceProvider();
        Assert.IsNotNull(sp.GetService<IMediator>());
        Assert.IsNotNull(sp.GetService<ISender>());
        Assert.IsNotNull(sp.GetService<IPublisher>());
    }
}