namespace ThabeSoft.Mediator.UnitTests.Datas;

public sealed class RequestHandler : IRequestHandler<Request>
{
    public ValueTask HandleAsync(Request request, CancellationToken cancellationToken = default) => default;
}
