using Microsoft.Extensions.DependencyInjection;

namespace ThabeSoft.Mediator.Tests;


/// <summary>
/// 中介者测试基类
/// </summary>
public abstract class MediatorTestBase
{
    private IServiceProvider _provider = default!;
    private IServiceScope _scope = default!;

    public TestContext TestContext { get; set; } = default!;
    protected static IMediator Mediator { get; private set; } = default!;


    [TestInitialize]
    public void Setup()
    {
        var services = new ServiceCollection();
        services.AddMediator();
        services.AddMediatorHandlers();

        _provider = services.BuildServiceProvider();
        _scope = _provider.CreateScope();
        Mediator = _provider.GetRequiredService<IMediator>();
    }

    [TestCleanup]
    public void Cleanup()
    {
        _scope.Dispose();
    }
}
