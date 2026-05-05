using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Runtime.CompilerServices;
using ThabeSoft.Mediator;


#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace Microsoft.Extensions.DependencyInjection;

#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配


public static class DependencyInjectionExtensions
{
    private static readonly ConditionalWeakTable<IServiceCollection, List<Action<IHandlerRegistry>>> _registrationsMap = new();

    /// <summary>
    /// 添加中介者
    /// </summary>
    public static IServiceCollection AddMediator(this IServiceCollection services)
    {
        services.TryAddScoped<IMediator, Mediator>();
        return services;
    }

    public static IServiceCollection AddMediatorCommandHandler<TCommand>(this IServiceCollection services) where TCommand : ICommand
    {
        services.AddKeyedSingleton<CommandHandleDelegate>(typeof(TCommand), (services, key)=>
        {
            return async (command, ct) =>
            {
                var handler = services.GetRequiredService<ICommandHandler<TCommand>>();
                await handler.HandleAsync((TCommand)command, ct);
            };
        });

        return services;
    }

    public static IServiceCollection AddMediatorCommandHandler<TCommand, TResult>(this IServiceCollection services) where TCommand : ICommand<TResult>
    {
        services.TryAddKeyedScoped<CommandHandleDelegate<TResult>>(typeof(TCommand), (services, key) =>
        {
            return async (command, ct) =>
            {
                var handler = services.GetRequiredService<ICommandHandler<TCommand, TResult>>();
                return await handler.HandleAsync((TCommand)command, ct);
            };
        });

        return services;
    }

    public static IServiceCollection AddMediatorQueryHandler<TQuery, TResult>(this IServiceCollection services) where TQuery : IQuery<TResult>
    {
        services.AddKeyedSingleton<QueryHandleDelegate<TResult>>(typeof(TQuery), (services, key)=>
        {
            return async (query, ct) =>
            {
                var handler = services.GetRequiredService<IQueryHandler<TQuery, TResult>>();
                return await handler.HandleAsync((TQuery)query, ct);
            };
        });

        return services;
    }
    public static IServiceCollection AddMediatorEventHandler<TEvent>(this IServiceCollection services) where TEvent : IEvent
    {
        services.AddKeyedSingleton<EventHandleDelegate>(typeof(TEvent), (services, key)=>
        {
            return async (@event, ct) =>
            {
                var handler = services.GetRequiredService<IEventHandler<TEvent>>();
                await handler.HandleAsync((TEvent)@event, ct);
            };
        });

        return services;
    }


    private static List<Action<IHandlerRegistry>> GetRegistrations(this IServiceCollection services)
    {
        if(!_registrationsMap.TryGetValue(services, out var registrations))
        {
            registrations = [];
            _registrationsMap.Add(services, registrations);
        }

        return registrations;
    }
}