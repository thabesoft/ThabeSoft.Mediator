namespace ThabeSoft.Mediator.UnitTests.Datas;

public sealed class RequestResponseHandler : IRequestHandler<RequestResponse, Response>
{
    public ValueTask<Response> HandleAsync(RequestResponse request, CancellationToken cancellationToken = default)
    {
        return ValueTask.FromResult<Response>(default!);
    }
}
