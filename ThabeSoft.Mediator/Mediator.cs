using System.Threading;
using System.Threading.Tasks;


namespace ThabeSoft.Mediator
{
    internal sealed class Mediator : IMediator
    {
        private readonly IEventDispatcher _eventDispatcher;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IQueryDispatcher _queryDispatcher;

        public Mediator(
            IEventDispatcher eventDispatcher,
            ICommandDispatcher commandDispatcher,
            IQueryDispatcher queryDispatcher
        )
        {
            _eventDispatcher = eventDispatcher;
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;
        }

        public async Task SendAsync(ICommand command, CancellationToken cancellationToken)
        {
            await _commandDispatcher.DispatchAsync(command, cancellationToken);
        }

        public async Task<TResult> SendAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken)
        {
            return await _commandDispatcher.DispatchAsync(command, cancellationToken);
        }

        public async Task<TResult> QueryAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken)
        {
            return await _queryDispatcher.DispatchAsync(query, cancellationToken);
        }

        public async Task PublishAsync(IEvent @event, CancellationToken cancellationToken)
        {
            await _eventDispatcher.DispatchAsync(@event, cancellationToken);
        }
    }
}