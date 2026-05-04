using Microsoft.Extensions.DependencyInjection;
using ThabeSoft.Mediator;

namespace Test;


public abstract class TestBase
{
    private IServiceProvider _provider = default!;
    private IServiceScope _scope = default!;

    public TestContext TestContext { get; set; }
    protected IMediator Mediator { get; private set; } = default!;


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
        if (_provider is IDisposable disposable)
            disposable.Dispose();

        _scope.Dispose();
    }
}
