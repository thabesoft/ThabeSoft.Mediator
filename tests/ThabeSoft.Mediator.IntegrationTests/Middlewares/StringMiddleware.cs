namespace ThabeSoft.Mediator.IntegrationTests.Middlewares;


public sealed class StringMiddleware<TReq> : IMiddleware<TReq>
{
    public ValueTask InvokeAsync(TReq req, RequestHandlerDelegateObsolete<TReq> next, CancellationToken cancellationToken)
    {
        return next(req, cancellationToken);
    }
}