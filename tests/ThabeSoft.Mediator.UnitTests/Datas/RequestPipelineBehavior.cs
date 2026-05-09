namespace ThabeSoft.Mediator.UnitTests.Datas;

public sealed class RequestPipelineBehavior : IRequestPipelineBehavior<Request>
{
    public ValueTask InvokeAsync(Request request, HandlerDelegate next, CancellationToken cancellationToken = default)
    {
        return next(cancellationToken);
    }
}
