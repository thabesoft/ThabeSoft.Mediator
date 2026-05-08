//using MediatR;

//namespace ThabeSoft.Mediator.Benchmark.Middlewares;

//public sealed class TransactionMiddleware<TRequest, TResponse> : IMiddleware<TRequest, TResponse>, IPipelineBehavior<TRequest, TResponse>
//{
//    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
//    {
//        return next(cancellationToken);
//    }

//    public ValueTask<TResponse> InvokeAsync(TRequest message, NextMiddleware<TRequest, TResponse> next, CancellationToken cancellationToken = default)
//    {
//        return next(message, cancellationToken);
//    }
//}

//public sealed class ValidationMiddleware<TRequest, TResponse> : IMiddleware<TRequest, TResponse>, IPipelineBehavior<TRequest, TResponse>
//{
//    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
//    {
//        return next(cancellationToken);
//    }

//    public ValueTask<TResponse> InvokeAsync(TRequest message, NextMiddleware<TRequest, TResponse> next, CancellationToken cancellationToken = default)
//    {
//        return next(message, cancellationToken);
//    }
//}

//public sealed class AuthorizationMiddleware<TRequest, TResponse> : IMiddleware<TRequest, TResponse>, IPipelineBehavior<TRequest, TResponse>
//{
//    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
//    {
//        return next(cancellationToken);
//    }

//    public ValueTask<TResponse> InvokeAsync(TRequest message, NextMiddleware<TRequest, TResponse> next, CancellationToken cancellationToken = default)
//    {
//        return next(message, cancellationToken);
//    }
//}


//public sealed class CachingMiddleware<TRequest, TResponse> : IMiddleware<TRequest, TResponse>, IPipelineBehavior<TRequest, TResponse>
//{
//    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
//    {
//        return next(cancellationToken);
//    }

//    public ValueTask<TResponse> InvokeAsync(TRequest message, NextMiddleware<TRequest, TResponse> next, CancellationToken cancellationToken = default)
//    {
//        return next(message, cancellationToken);
//    }
//}


//public sealed class MetricsMiddleware<TRequest, TResponse> : IMiddleware<TRequest, TResponse>, IPipelineBehavior<TRequest, TResponse>
//{
//    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
//    {
//        return next(cancellationToken);
//    }

//    public ValueTask<TResponse> InvokeAsync(TRequest message, NextMiddleware<TRequest, TResponse> next, CancellationToken cancellationToken = default)
//    {
//        return next(message, cancellationToken);
//    }
//}

//public sealed class RetryMiddleware<TRequest, TResponse> : IMiddleware<TRequest, TResponse>, IPipelineBehavior<TRequest, TResponse>
//{
//    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
//    {
//        return next(cancellationToken);
//    }

//    public ValueTask<TResponse> InvokeAsync(TRequest message, NextMiddleware<TRequest, TResponse> next, CancellationToken cancellationToken = default)
//    {
//        return next(message, cancellationToken);
//    }
//}

//public sealed class CircuitBreakerMiddleware<TRequest, TResponse> : IMiddleware<TRequest, TResponse>, IPipelineBehavior<TRequest, TResponse>
//{
//    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
//    {
//        return next(cancellationToken);
//    }

//    public ValueTask<TResponse> InvokeAsync(TRequest message, NextMiddleware<TRequest, TResponse> next, CancellationToken cancellationToken = default)
//    {
//        return next(message, cancellationToken);
//    }
//}

//public sealed class TracingMiddleware<TRequest, TResponse> : IMiddleware<TRequest, TResponse>, IPipelineBehavior<TRequest, TResponse>
//{
//    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
//    {
//        return next(cancellationToken);
//    }

//    public ValueTask<TResponse> InvokeAsync(TRequest message, NextMiddleware<TRequest, TResponse> next, CancellationToken cancellationToken = default)
//    {
//        return next(message, cancellationToken);
//    }
//}

//public sealed class CompressionMiddleware<TRequest, TResponse> : IMiddleware<TRequest, TResponse>, IPipelineBehavior<TRequest, TResponse>
//{
//    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
//    {
//        return next(cancellationToken);
//    }

//    public ValueTask<TResponse> InvokeAsync(TRequest message, NextMiddleware<TRequest, TResponse> next, CancellationToken cancellationToken = default)
//    {
//        return next(message, cancellationToken);
//    }
//}

//public sealed class EncryptionMiddleware<TRequest, TResponse> : IMiddleware<TRequest, TResponse>, IPipelineBehavior<TRequest, TResponse>
//{
//    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
//    {
//        return next(cancellationToken);
//    }

//    public ValueTask<TResponse> InvokeAsync(TRequest message, NextMiddleware<TRequest, TResponse> next, CancellationToken cancellationToken = default)
//    {
//        return next(message, cancellationToken);
//    }
//}