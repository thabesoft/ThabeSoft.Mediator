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
        public IServiceCollection AddMediator<TMediator>(ServiceLifetime lifetime)
            where TMediator : class, IMediator
        {
            var key = typeof(TMediator);

            services.RemoveAll<IMediator>();
            services.RemoveAll<ISender>();
            services.RemoveAll<IPublisher>();


            services.Add(new ServiceDescriptor(typeof(IMediator), typeof(Mediator), lifetime));
            services.Add(new ServiceDescriptor(typeof(ISender), sp => sp.GetRequiredService<IMediator>(), lifetime));
            services.Add(new ServiceDescriptor(typeof(IPublisher), sp => sp.GetRequiredService<IMediator>(), lifetime));

            return services;
        }

        /// <summary>
        /// 添加默认中介者
        /// </summary>
        public IServiceCollection AddMediator(ServiceLifetime lifetime)
        {
            return services.AddMediator<Mediator>(lifetime);
        }


        /// <summary>
        /// 删除所有业务和实现类型匹配的
        /// </summary>
        private void RemoveAll<TService, TImplementation>()
        {
            var service_type = typeof(TService);
            var implementation_type = typeof(TImplementation);
            var remove_items = services.Where(x => x.ServiceType == service_type && x.ImplementationType == implementation_type);

            foreach (var descriptor in remove_items.ToArray())
            {
                services.Remove(descriptor);
            }
        }
    }
}