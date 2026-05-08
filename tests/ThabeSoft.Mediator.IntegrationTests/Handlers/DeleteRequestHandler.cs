using ThabeSoft.Mediator.IntegrationTests.Messages;

namespace ThabeSoft.Mediator.IntegrationTests.Handlers;


public class DeleteRequestHandler : IRequestHandler<DeleteRequest>
{
    public ValueTask HandleAsync(DeleteRequest request, CancellationToken ct)
    {
        return default;
    }
}
