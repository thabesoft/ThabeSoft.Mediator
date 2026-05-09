using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Runtime.CompilerServices;
using ThabeSoft.Mediator;
using ThabeSoft.Mediator.DependencyInjection;


#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配


public static class DependencyInjectionExtensions
{
    /// <summary>
    /// 所有业务下的配置
    /// </summary>
    private readonly static ConditionalWeakTable<IServiceCollection, DescriptorCollection> _allOptions = new();


    extension(IServiceCollection services)
    {
        /// <summary>
        /// 配置中介者
        /// </summary>
        public IServiceCollection ConfigureMediator(Action<IDescriptorCollection> optionsAction)
        {
            if (!_allOptions.TryGetValue(services, out var options))
            {
                options = new DescriptorCollection();
                _allOptions.Add(services, options);
            }

            optionsAction?.Invoke(options);
            options.SyncToServiceCollection(services);


            return services;
        }

        /// <summary>
        /// 添加自定义中介者
        /// </summary>
        public IServiceCollection AddMediator<TMediator>(ServiceLifetime lifetime = ServiceLifetime.Scoped)
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
        public IServiceCollection AddMediator(ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            return services.AddMediator<Mediator>(lifetime);
        }
    }
}