using Microsoft.Extensions.DependencyInjection;

namespace ThabeSoft.Mediator.DependencyInjection;

public class SyntaxTest
{
    public static void Hanlder(IDescriptorCollection option)
    {
        option.Notifications().Singleton();
        option.Requests().Scoped();

        option.Scoped().Singleton();
        option.Transient().Singleton();

        option.Default(ServiceLifetime.Singleton);
        option.WithLifetime(LifetimeKind.Singleton | LifetimeKind.Scoped).Except();
    }

    public static void Middleware(IDescriptorCollection option)
    {
        option.Requests().Singleton();
    }
}
