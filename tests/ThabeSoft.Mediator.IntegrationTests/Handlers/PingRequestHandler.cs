using ThabeSoft.Mediator.Tests.Messages;

namespace ThabeSoft.Mediator.IntegrationTests.Handlers;

public class PingRequestHandler : IRequestHandler<PingRequest, PongResponse>
{
    public ValueTask<PongResponse> HandleAsync(PingRequest request, CancellationToken ct)
    {
        return ValueTask.FromResult(new PongResponse(request.PingId, "Pong"));
    }
}