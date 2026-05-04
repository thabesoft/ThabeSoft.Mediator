using Microsoft.Extensions.DependencyInjection;
using ThabeSoft.Mediator;
using ThabeSoft.Mediator.Warppers;

namespace Test;


public record Command : ICommand;
public class CommandHandler : ICommandHandler<Command>
{
    public Task HandleAsync(Command command, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}


public record ResponseCommand : ICommand<ResponseCommandResult>;
public record ResponseCommandResult;
public class ResponseCommandHandler : ICommandHandler<ResponseCommand, ResponseCommandResult>
{
    public Task<ResponseCommandResult> HandleAsync(ResponseCommand command, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}

public record Query : IQuery<QueryResult>;
public record QueryResult;
public class QueryHandler : IQueryHandler<Query, QueryResult>
{
    public Task<QueryResult> HandleAsync(Query query, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}



public record Event : IEvent;
public class EventHandler : IEventHandler<Event>
{
    public Task HandleAsync(Event @event, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}