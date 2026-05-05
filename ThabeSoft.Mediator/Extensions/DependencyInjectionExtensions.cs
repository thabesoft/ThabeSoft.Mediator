using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Runtime.CompilerServices;
using ThabeSoft.Mediator;


#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace Microsoft.Extensions.DependencyInjection;

#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配


public static class DependencyInjectionExtensions
{
    /// <summary>
    /// 添加中介者
    /// </summary>
    public static IServiceCollection AddMediator(this IServiceCollection services)
    {
        services.TryAddScoped<IMediator, Mediator>();
        return services;
    }
}