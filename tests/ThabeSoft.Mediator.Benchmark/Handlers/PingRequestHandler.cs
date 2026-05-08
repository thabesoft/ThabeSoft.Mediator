using ThabeSoft.Mediator.Benchmark.Messages;

namespace ThabeSoft.Mediator.Benchmark.Handlers;



public class PingRequestHandler : RequestHandlerBase<PingRequest, PongResponse>
{
    protected override Task<PongResponse> HandleAsync(PingRequest request, CancellationToken cancellationToken)
    {
        var result = new PongResponse($"Pong: {DateTime.Now}");
        return Task.FromResult(result);
    }
    protected override ValueTask<PongResponse> ValueHandleAsync(PingRequest request, CancellationToken cancellationToken)
    {
        var result = new PongResponse($"Pong: {DateTime.Now}");
        return ValueTask.FromResult(result);
    }
}