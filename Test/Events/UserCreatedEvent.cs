using ThabeSoft.Mediator;

namespace Test.Events;


public record UserCreatedEvent(int UserId, string Name) : IEvent;
