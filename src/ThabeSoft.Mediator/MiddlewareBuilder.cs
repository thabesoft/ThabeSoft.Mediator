namespace ThabeSoft.Mediator;


/// <summary>
/// 中间件构建器
/// </summary>
public static class MiddlewareBuilder
{
    public static MiddlewareDelegate<TRequest> BuildRequest<TRequest>(IMiddleware<TRequest>[] middlewares)
        where TRequest : IRequest
    {
        var a = new StateMachine<TRequest>(middlewares);
        return a.FirstMethod;
    }
    public static MiddlewareDelegate<TRequest, TResponse> BuildRequest<TRequest, TResponse>(IMiddleware<TRequest, TResponse>[] middlewares)
        where TRequest : IRequest<TResponse>
    {
        var a = new StateMachine<TRequest, TResponse>(middlewares);
        return a.FirstMethod;
    }
    

    private sealed class StateMachine<TRequest, TResponse>(IMiddleware<TRequest, TResponse>[] middlewares)
        where TRequest : IRequest<TResponse>
    {
        private int _index = 0;
        private NextMiddleware<TRequest, TResponse> _handler = null!;
        private NextMiddleware<TRequest, TResponse> _next = null!;

        private ValueTask<TResponse> Call(TRequest message, CancellationToken ct)
        {
            if (_index < middlewares.Length)
            {
                var middleware = middlewares[_index];
                _index++;
                return middleware.InvokeAsync(message, _handler, ct);
            }

            return _next(message, ct);
        }

        public ValueTask<TResponse> FirstMethod(TRequest message, NextMiddleware<TRequest, TResponse> next, CancellationToken ct)
        {
            _next = next;
            _index = 0;
            _handler = Call;
            return _handler.Invoke(message, ct);
        }
    }

    private sealed class StateMachine<TRequest>(IMiddleware<TRequest>[] middlewares)
        where TRequest : IRequest
    {
        private int _index = 0;
        private NextMiddleware<TRequest> _handler = null!;
        private NextMiddleware<TRequest> _next = null!;

        private ValueTask Call(TRequest message, CancellationToken ct)
        {
            if (_index < middlewares.Length)
            {
                var middleware = middlewares[_index];
                _index++;
                return middleware.InvokeAsync(message, _handler, ct);
            }

            return _next(message, ct);
        }

        public ValueTask FirstMethod(TRequest message, NextMiddleware<TRequest> next, CancellationToken ct)
        {
            _next = next;
            _index = 0;
            _handler = Call;
            return _handler.Invoke(message, ct);
        }
    }
}