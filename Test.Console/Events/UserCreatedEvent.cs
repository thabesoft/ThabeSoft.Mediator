using ThabeSoft.Mediator;

namespace Test.Console.Events;


public record UserCreatedEvent(int UserId, string Name) : IEvent;
