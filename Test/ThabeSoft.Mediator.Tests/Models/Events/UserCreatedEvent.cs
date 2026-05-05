using ThabeSoft.Mediator;

namespace Test.Models.Events;


public record UserCreatedEvent(int UserId, string Name) : IEvent;


public class UserCreatedEventHandler : IEventHandler<UserCreatedEvent>
{
    public ValueTask HandleAsync(UserCreatedEvent @event, CancellationToken ct)
    {
        Console.WriteLine($"User created: {@event.Name}");
        return ValueTask.CompletedTask;
    }
}