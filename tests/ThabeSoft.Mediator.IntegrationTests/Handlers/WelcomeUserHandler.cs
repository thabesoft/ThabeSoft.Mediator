using ThabeSoft.Mediator.Tests.Messages;

namespace ThabeSoft.Mediator.IntegrationTests.Handlers;

public sealed class WelcomeUserHandler : INotificationHandler<UserCreatedNotification>
{
    public ValueTask HandleAsync(UserCreatedNotification notification, CancellationToken ct)
    {
        return ValueTask.CompletedTask;
    }
}
