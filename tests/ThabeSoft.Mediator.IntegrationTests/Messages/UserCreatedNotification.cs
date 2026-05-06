namespace ThabeSoft.Mediator.Tests.Messages;


public record UserCreatedNotification(int UserId, string Name) : INotification;