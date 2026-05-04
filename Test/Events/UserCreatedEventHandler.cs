using ThabeSoft.Mediator;

namespace Test.Events;

public class UserCreatedEventHandler : IEventHandler<UserCreatedEvent>
{
    public Task HandleAsync(UserCreatedEvent @event, CancellationToken ct)
    {
        Console.WriteLine($"User created: {@event.Name}");
        return Task.CompletedTask;
    }
}
