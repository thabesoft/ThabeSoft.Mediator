using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;

namespace ThabeSoft.Mediator;


internal sealed class Mediator(IServiceProvider services) : IMediator
{
    public ValueTask SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default) where TCommand : ICommand
    {
        return CommandHandlerSlot<TCommand>.Handler.Invoke(services, command, cancellationToken);
    }
    public ValueTask<TResult> SendAsync<TCommand, TResult>(TCommand command, CancellationToken cancellationToken = default) where TCommand : ICommand<TResult>
    {
        return CommandHandlerSlot<TCommand, TResult>.Handler.Invoke(services, command, cancellationToken);
    }
    public ValueTask<TResult> QueryAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default) where TQuery : IQuery<TResult>
    {
        return QueryHandlerSlot<TQuery, TResult>.Handler.Invoke(services, query, cancellationToken);
    }
    public ValueTask PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : IEvent
    {
        var handlers = EventHandlerSlot<TEvent>.GetHandlers(services);
        var handler_length = handlers.Length;

        if (handler_length <= 0) return default;
        if (handler_length == 1) return handlers[0].Invoke(@event, cancellationToken);

        var tasks = new Task[handler_length];
        for (int i = 0; i < handler_length; i++)
            tasks[i] = handlers[i].Invoke(@event, cancellationToken).AsTask();

        return new ValueTask(Task.WhenAll(tasks));
    }
}


internal static class CommandHandlerSlot<TCommand> where TCommand : ICommand
{
    public delegate ValueTask Delegate(IServiceProvider services, TCommand command, CancellationToken cancellationToken);

    public static Delegate Handler = async (services, command, ct) =>
    {
        var handler = services.GetRequiredService<ICommandHandler<TCommand>>();
        await handler.HandleAsync(command, ct);
    };
}

internal static class CommandHandlerSlot<TCommand, TResult> where TCommand : ICommand<TResult>
{
    public delegate ValueTask<TResult> Delegate(IServiceProvider services, TCommand command, CancellationToken cancellationToken);

    public static Delegate Handler = async (services, command, ct) =>
    {
        var handler = services.GetRequiredService<ICommandHandler<TCommand, TResult>>();
        return await handler.HandleAsync(command, ct);
    };
}

internal static class QueryHandlerSlot<TQuery, TResult> where TQuery : IQuery<TResult>
{
    public delegate ValueTask<TResult> Delegate(IServiceProvider services, TQuery query, CancellationToken cancellationToken);

    public static Delegate Handler = async (services, command, ct) =>
    {
        var handler = services.GetRequiredService<IQueryHandler<TQuery, TResult>>();
        return await handler.HandleAsync(command, ct);
    };
}

internal static class EventHandlerSlot<TEvent> where TEvent : IEvent
{
    private static readonly ConditionalWeakTable<IServiceProvider, Delegate[]> _handlerMap = new();


    public delegate ValueTask Delegate(TEvent @event, CancellationToken cancellationToken);

    public static Delegate[] GetHandlers(IServiceProvider services)
    {
        if (_handlerMap.TryGetValue(services, out var handlers))
            return handlers;

        handlers = [.. services.GetServices<IEventHandler<TEvent>>().Select(handler => new Delegate(handler.HandleAsync))];
        _handlerMap.Add(services, handlers);

        return handlers;
    }
}