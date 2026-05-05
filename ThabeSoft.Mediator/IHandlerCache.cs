using System.Data;

namespace ThabeSoft.Mediator;


public delegate Task CommandHandleDelegate(ICommand command, CancellationToken cancellationToken);
public delegate Task<TResult> CommandHandleDelegate<TResult>(ICommand<TResult> command, CancellationToken cancellationToken);
public delegate Task<TResult> QueryHandleDelegate<TResult>( IQuery<TResult> query, CancellationToken cancellationToken);
public delegate Task EventHandleDelegate(IEvent @event, CancellationToken cancellationToken);


public interface IHandlerRegistry
{
    void RegisterCommand<TCommand>(CommandHandleDelegate handler) where TCommand : ICommand;
    void RegisterCommand<TCommand, TResult>(CommandHandleDelegate<TResult> handler) where TCommand : ICommand<TResult>;
    void RegisterQuery<TQuery, TResult>(QueryHandleDelegate<TResult> handler) where TQuery : IQuery<TResult>;
    void RegisterEvent<TEvent>(EventHandleDelegate handler) where TEvent : IEvent;
}

public interface IHandlerProvider
{
    CommandHandleDelegate GetCommand(Type commandType);
    CommandHandleDelegate<TResult> GetCommand<TResult>(Type commandType);
    QueryHandleDelegate<TResult> GetQuery<TResult>(Type queryType);
    IReadOnlyCollection<EventHandleDelegate> GetEvents(Type @eventType);
}

internal sealed class HandlerStorage : IHandlerRegistry, IHandlerProvider
{
    private readonly Dictionary<Type, Delegate> _commandHandlers = [];
    private readonly Dictionary<Type, Delegate> _responseCommandHandlers = [];
    private readonly Dictionary<Type, Delegate> _queryHandlers = [];
    private readonly Dictionary<Type, List<Delegate>> _eventHandlers = [];


    public void RegisterCommand<TCommand>(CommandHandleDelegate handler) where TCommand : ICommand
    {
        _commandHandlers[typeof(TCommand)] = handler;
    }
    public void RegisterCommand<TCommand, TResult>(CommandHandleDelegate<TResult> handler) where TCommand : ICommand<TResult>
    {
        _responseCommandHandlers[typeof(TCommand)] = handler;
    }
    public void RegisterQuery<TQuery, TResult>(QueryHandleDelegate<TResult> handler) where TQuery : IQuery<TResult>
    {
        _queryHandlers[typeof(TQuery)] = handler;
    }
    public void RegisterEvent<TEvent>(EventHandleDelegate handler) where TEvent : IEvent
    {
        var event_type = typeof(TEvent);

        if (!_eventHandlers.TryGetValue(event_type, out var handlers))
        {
            handlers = [];
            _eventHandlers[event_type] = handlers;
        }

        handlers.Add(handler);
    }


    public CommandHandleDelegate GetCommand(Type commandType)
    {
        if(_commandHandlers.TryGetValue(commandType, out var @delegate)) return (CommandHandleDelegate)@delegate;
        throw new NotSupportedException($"{commandType} is not register");
    }

    public CommandHandleDelegate<TResult> GetCommand<TResult>(Type commandType)
    {
        if (_responseCommandHandlers.TryGetValue(commandType, out var @delegate)) return (CommandHandleDelegate<TResult>)@delegate;
        throw new NotSupportedException($"{commandType} is not register");
    }

    public QueryHandleDelegate<TResult> GetQuery<TResult>(Type queryType)
    {
        if (_queryHandlers.TryGetValue(queryType, out var @delegate)) return (QueryHandleDelegate<TResult>)@delegate;
        throw new NotSupportedException($"{queryType} is not register");
    }

    public IReadOnlyCollection<EventHandleDelegate> GetEvents(Type eventType)
    {
        if (_eventHandlers.TryGetValue(eventType, out var @delegates)) return [.. @delegates.OfType<EventHandleDelegate>()];
        throw new NotSupportedException($"{eventType} is not register");
    }
}