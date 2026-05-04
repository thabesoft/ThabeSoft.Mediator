using Microsoft.Extensions.DependencyInjection;
using ThabeSoft.Mediator.Warppers;

namespace ThabeSoft.Mediator;


internal sealed class Mediator(IServiceProvider services) : IMediator
{
    // 每种命令对应的处理器
    private readonly Dictionary<Type, ICommandHandlerWarpper> _nonResultCommandHandlerWarppers =
        services.GetServices<ICommandHandlerWarpper>()
        .ToDictionary(x => x.MessageType);

    // 每种结果命令对应的处理器
    private readonly Dictionary<Type, IResponseCommandHandlerWarpper> _resultCommandHandlerWarppers =
        services.GetServices<IResponseCommandHandlerWarpper>()
        .ToDictionary(x => x.MessageType);

    // 每种请求对应的处理器
    private readonly Dictionary<Type, IQueryHandlerWarpper> _queryHandlerWarppers =
        services.GetServices<IQueryHandlerWarpper>()
        .ToDictionary(x => x.MessageType);

    // 每种事件对应的处理器
    private readonly Dictionary<Type, IEventHandlerWarpper[]> _eventHandlerWarppers =
        services.GetServices<IEventHandlerWarpper>()
        .GroupBy(x => x.MessageType)
        .ToDictionary(x => x.First().MessageType, v => v.ToArray());


    public async Task SendAsync(ICommand command, CancellationToken cancellationToken)
    {
        var message_type = command.GetType();

        if (!_nonResultCommandHandlerWarppers.TryGetValue(message_type, out var warpper))
        {
            throw new NotSupportedException("");
        }

        await warpper.HandleAsync(command, cancellationToken);
    }

    public async Task<TResult> SendAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken)
    {
        var message_type = command.GetType();

        if (!_resultCommandHandlerWarppers.TryGetValue(message_type, out var warpper))
        {
            throw new NotSupportedException("");
        }

        return await warpper.HandleAsync(command, cancellationToken);
    }

    public async Task<TResult> QueryAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken)
    {
        var message_type = query.GetType();

        if (!_queryHandlerWarppers.TryGetValue(message_type, out var warpper))
        {
            throw new NotSupportedException();
        }

        return await warpper.HandleAsync(query, cancellationToken);
    }

    public async Task PublishAsync(IEvent @event, CancellationToken cancellationToken)
    {
        var message_type = @event.GetType();

        if (!_eventHandlerWarppers.TryGetValue(message_type, out var warppers))
        {
            throw new NotSupportedException();
        }

        foreach (var warpper in warppers)
        {
            await warpper.HandleAsync(@event, cancellationToken);
        }
    }
}