//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.DependencyInjection.Extensions;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using ThabeSoft.Mediator.SourceGenerator.Warppers;
//using ThabeSoft.Mediator.Warppers;

//namespace ThabeSoft.Mediator;


//public record Command : ICommand;
//public class CommandHandler : ICommandHandler<Command>
//{
//    public Task HandleAsync(Command command, CancellationToken cancellationToken = default)
//    {
//        throw new NotImplementedException();
//    }
//}


//public record ResponseCommand : ICommand<ResponseCommandResult>;
//public record ResponseCommandResult;
//public class ResponseCommandHandler : ICommandHandler<ResponseCommand, ResponseCommandResult>
//{
//    public Task<ResponseCommandResult> HandleAsync(ResponseCommand command, CancellationToken cancellationToken = default)
//    {
//        throw new NotImplementedException();
//    }
//}

//public record Query : IQuery<QueryResult>;
//public record QueryResult;
//public class QueryHandler : IQueryHandler<Query, QueryResult>
//{
//    public Task<QueryResult> HandleAsync(Query query, CancellationToken cancellationToken = default)
//    {
//        throw new NotImplementedException();
//    }
//}



//public record Event : IEvent;
//public class EventHandler : IEventHandler<Event>
//{
//    public Task HandleAsync(Event @event, CancellationToken cancellationToken = default)
//    {
//        throw new NotImplementedException();
//    }
//}


//internal sealed class ThabeSoftMediatorQueryHandler(IServiceProvider services) : IQueryHandlerWarpper
//{
//    public Type MessageType { get; } = typeof(ThabeSoft.Mediator.Query);

//    public async Task<TResult> HandleAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default)
//    {
//        if (typeof(ThabeSoft.Mediator.QueryResult) != typeof(TResult)) throw new NotSupportedException();
//        if (query is not Query q) throw new NotSupportedException();

//        var handler = services.GetRequiredService<IQueryHandler<ThabeSoft.Mediator.Query, ThabeSoft.Mediator.QueryResult>>();
        
//        // 我能确定下面不会有else了怎么告诉编译器
//        if (await handler.HandleAsync(q, cancellationToken) is TResult result) return result;
//        throw new InvalidOperationException("类型匹配已通过，不应执行此处");
//    }
//}