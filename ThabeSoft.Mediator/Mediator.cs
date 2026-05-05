using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

namespace ThabeSoft.Mediator;


internal sealed class Mediator(IServiceProvider services) : IMediator
{
    public async Task SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default) where TCommand : ICommand
    {
        await CommandHandlerSlot<TCommand>.Handler.Invoke(services, command, cancellationToken);
    }
    public async Task<TResult> SendAsync<TCommand, TResult>(TCommand command, CancellationToken cancellationToken = default) where TCommand : ICommand<TResult>
    {
        return await CommandHandlerSlot<TCommand, TResult>.Handler.Invoke(services, command, cancellationToken);
    }
    public async Task<TResult> QueryAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default) where TQuery : IQuery<TResult>
    {
        return await QueryHandlerSlot<TQuery, TResult>.Handler.Invoke(services, query, cancellationToken);
    }
    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : IEvent
    {
        await Task.WhenAll(EventHandlerSlot<TEvent>.GetHandlers(services).Select(x => x.Invoke(@event, cancellationToken)));
    }
}


internal static class CommandHandlerSlot<TCommand> where TCommand : ICommand
{
    public delegate Task Delegate(IServiceProvider services, TCommand command, CancellationToken cancellationToken);

    public static Delegate Handler = async (services, command, ct) =>
    {
        var handler = services.GetRequiredService<ICommandHandler<TCommand>>();
        await handler.HandleAsync(command, ct);
    };
}

internal static class CommandHandlerSlot<TCommand, TResult> where TCommand : ICommand<TResult>
{
    public delegate Task<TResult> Delegate(IServiceProvider services, TCommand command, CancellationToken cancellationToken);

    public static Delegate Handler = async (services, command, ct) =>
    {
        var handler = services.GetRequiredService<ICommandHandler<TCommand, TResult>>();
        return await handler.HandleAsync(command, ct);
    };
}

internal static class QueryHandlerSlot<TQuery, TResult> where TQuery : IQuery<TResult>
{
    public delegate Task<TResult> Delegate(IServiceProvider services, TQuery query, CancellationToken cancellationToken);

    public static Delegate Handler = async (services, command, ct) =>
    {
        var handler = services.GetRequiredService<IQueryHandler<TQuery, TResult>>();
        return await handler.HandleAsync(command, ct);
    };
}

internal static class EventHandlerSlot<TEvent> where TEvent : IEvent
{
    private static readonly ConcurrentDictionary<IServiceProvider, IReadOnlyCollection<Delegate>> _handlerMap = [];


    public delegate Task Delegate(TEvent @event, CancellationToken cancellationToken);

    public static IReadOnlyCollection<Delegate> GetHandlers(IServiceProvider services)
    {
        return _handlerMap.GetOrAdd(services, x =>
        {
            return [.. services
                .GetServices<IEventHandler<TEvent>>()
                .Select(handler => new Delegate(
                    (@event, ct) => handler.HandleAsync(@event, ct))
            )];
        });
    }
}