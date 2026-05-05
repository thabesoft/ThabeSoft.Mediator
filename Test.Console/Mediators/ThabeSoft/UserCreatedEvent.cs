using ThabeSoft.Mediator;

namespace Test.Console.Mediators.ThabeSoft;


public record UserCreatedEvent(int UserId, string Name) : IEvent;


public class UserCreatedEventHandler : IEventHandler<UserCreatedEvent>
{
    public ValueTask HandleAsync(UserCreatedEvent @event, CancellationToken ct)
    {
        return ValueTask.CompletedTask;
    }
}