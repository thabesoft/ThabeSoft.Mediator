using ThabeSoft.Mediator;

namespace Test.Console.Events;

public class UserCreatedEventHandler : IEventHandler<UserCreatedEvent>
{
    public Task HandleAsync(UserCreatedEvent @event, CancellationToken ct)
    {
        return Task.CompletedTask;
    }
}
