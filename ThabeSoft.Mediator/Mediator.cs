using Microsoft.Extensions.DependencyInjection;

namespace ThabeSoft.Mediator;


internal sealed class Mediator(IServiceProvider services) : IMediator
{
    public async Task SendAsync(ICommand command, CancellationToken cancellationToken)
    {
        var handler = services.GetRequiredKeyedService<CommandHandleDelegate>(command.GetType());
        await handler.Invoke(command, cancellationToken);
    }

    public async Task<TResult> SendAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken)
    {
        var handler = services.GetRequiredKeyedService<CommandHandleDelegate<TResult>>(command.GetType());
        return await handler.Invoke(command, cancellationToken);
    }

    public async Task<TResult> QueryAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken)
    {
        var handler = services.GetRequiredKeyedService<QueryHandleDelegate<TResult>>(query.GetType());
        return await handler.Invoke(query, cancellationToken);
    }

    public async Task PublishAsync(IEvent @event, CancellationToken cancellationToken)
    {
        var handle_tasks = services.GetKeyedServices<EventHandleDelegate>(@event.GetType())
            .Select(x => x.Invoke(@event, cancellationToken));
        await Task.WhenAll(handle_tasks);
    }
}