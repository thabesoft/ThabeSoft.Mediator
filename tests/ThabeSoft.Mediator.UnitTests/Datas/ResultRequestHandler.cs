using ThabeSoft.Mediator.UnitTests.Requests;

namespace ThabeSoft.Mediator.UnitTests.Datas;

public sealed class ResultRequestHandler : IRequestHandler<ResultRequest, Response>
{
    public ValueTask<Response> HandleAsync(ResultRequest request, CancellationToken cancellationToken = default)
    {
        return ValueTask.FromResult<Response>(default!);
    }
}
