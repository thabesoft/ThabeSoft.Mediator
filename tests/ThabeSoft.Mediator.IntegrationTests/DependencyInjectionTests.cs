using Microsoft.Extensions.DependencyInjection;
using ThabeSoft.Mediator.Tests.Messages;

namespace ThabeSoft.Mediator.IntegrationTests;


[TestClass]
public class DependencyInjectionTests
{
    [TestMethod(DisplayName = "注册生成代码")]
    public void AddGeneratedMediator()
    {
        var services = new ServiceCollection();
        services.AddMediator();
        services.AddGeneratedMediator();

        var sp = services.BuildServiceProvider();
        Assert.IsNotNull(sp.GetService<IMediator>());
    }
}