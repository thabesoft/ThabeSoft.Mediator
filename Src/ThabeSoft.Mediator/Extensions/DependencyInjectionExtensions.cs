using Microsoft.Extensions.DependencyInjection.Extensions;
using ThabeSoft.Mediator;


#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配


public static partial class DependencyInjectionExtensions
{
    /// <summary>
    /// 添加自定义中介者
    /// </summary>
    public static IServiceCollection AddMediator<TMediator>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TMediator : class, IMediator
    {
        services.TryAdd(new ServiceDescriptor(typeof(IMediator), typeof(TMediator), lifetime));
        services.TryAdd(new ServiceDescriptor(typeof(ISender), sp => sp.GetRequiredService<IMediator>(), lifetime));
        services.TryAdd(new ServiceDescriptor(typeof(IPublisher), sp => sp.GetRequiredService<IMediator>(), lifetime));

        return services;
    }

    /// <summary>
    /// 添加默认中介者
    /// </summary>
    /// <param name="services"></param>
    /// <param name="lifetime"></param>
    /// <returns></returns>
    public static IServiceCollection AddMediator(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        return services.AddMediator<Mediator>(lifetime);
    }
}