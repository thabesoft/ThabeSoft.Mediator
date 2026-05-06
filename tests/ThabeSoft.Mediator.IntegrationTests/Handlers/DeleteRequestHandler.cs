using ThabeSoft.Mediator.Tests.Messages;

namespace ThabeSoft.Mediator.IntegrationTests.Handlers;

public class DeleteRequestHandler : IRequestHandler<DeleteRequest>
{
    public ValueTask HandleAsync(DeleteRequest request, CancellationToken ct)
    {
        return ValueTask.CompletedTask;
    }
}
