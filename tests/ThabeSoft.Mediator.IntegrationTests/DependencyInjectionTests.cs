using Microsoft.Extensions.DependencyInjection;
using ThabeSoft.Mediator.DependencyInjection;

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

        services.AddMediator(x =>
        {
            x.RequestHandler().Singleton();
            x.RequestHandler<TReuqest>().Except();
        });

        var sp = services.BuildServiceProvider();
        Assert.IsNotNull(sp.GetService<IMediator>());
    }
}