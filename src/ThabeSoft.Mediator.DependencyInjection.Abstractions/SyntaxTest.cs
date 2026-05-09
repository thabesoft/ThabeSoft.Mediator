#if DEBUG

using Microsoft.Extensions.DependencyInjection;

namespace ThabeSoft.Mediator.DependencyInjection;

internal class SyntaxTest
{
    public static void Hanlder(IDescriptorCollection option)
    {
        option.RequestHandler().Singleton();
        option.RequestHandler().Scoped();

        option.Scoped().Singleton();
        option.Transient().Singleton();

        option.Default(ServiceLifetime.Singleton);
        option.WithLifetime(LifetimeKind.Singleton | LifetimeKind.Scoped).Except();
    }

    public static void Middleware(IDescriptorCollection option)
    {
        option.RequestHandler().Singleton();
    }
}

#endif