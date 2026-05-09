namespace ThabeSoft.Mediator.UnitTests.Datas;

public sealed class RequestResponsePipelineBehavior : IRequestPipelineBehavior<RequestResponse, Response>
{
    public ValueTask<Response> InvokeAsync(RequestResponse request, HandlerDelegate<Response> next, CancellationToken cancellationToken = default)
    {
        return next(cancellationToken);
    }
}
