using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace ThabeSoft.Mediator;


/// <summary>
/// 发送者扩展
/// </summary>
public static class SenderExtensions
{
    private static readonly MethodInfo _requestMethod;
    private static readonly MethodInfo _requestResponseMethod;

    static SenderExtensions()
    {
        var sender_methods = typeof(ISender)
            .GetMethods()
            .Where(x => x.Name == nameof(ISender.SendAsync) && x.IsGenericMethod)
            .ToArray();

        // ValueTask SendAsync<TRequest>(TRequest request, CancellationToken cancellationToken);
        _requestMethod = sender_methods.First(x =>
        {
            var @params = x.GetParameters();
            if (@params.Length != 2) return false;

            if (!@params[0].ParameterType.IsGenericParameter) return false;
            if (@params[1].ParameterType != typeof(CancellationToken)) return false;
            if (x.ReturnType != typeof(ValueTask)) return false;

            return true;
        });

        // ValueTask<TResponse> SendAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken);
        _requestResponseMethod = sender_methods.First(x =>
        {
            var @params = x.GetParameters();
            if (@params.Length != 2) return false;

            if (!@params[0].ParameterType.IsGenericParameter) return false;
            if (@params[1].ParameterType != typeof(CancellationToken)) return false;
            if (!x.ReturnType.IsGenericType) return false;

            return true;
        });
    }


    extension(ISender sender)
    {
        public ValueTask SendUntypedAsync(IRequest request, CancellationToken cancellationToken = default)
        {
            if (request is null) throw new ArgumentNullException(nameof(request), "请求不可为空");

            var actual_request_type = request.GetType();

            var invoker = RequestCahce.Delegates.GetOrAdd(actual_request_type, type =>
            {
                // 获取方法信息
                var genericMethod = _requestMethod.MakeGenericMethod(type);

                // 构建
                var sender_param = Expression.Parameter(typeof(ISender), "sender");
                var request_param = Expression.Parameter(typeof(object), "request");
                var ct_param = Expression.Parameter(typeof(CancellationToken), "ct");

                // 转为实际请求类型
                var convertedRequest = Expression.Convert(request_param, type);
                // 构建委托
                var call = Expression.Call(sender_param, genericMethod, convertedRequest, ct_param);
                var lambda = Expression.Lambda<RequestCahce.Delegate>(call, sender_param, request_param, ct_param);
                return lambda.Compile();
            });

            return invoker.Invoke(sender, request, cancellationToken);
        }
        public ValueTask<TResponse> SendUntypedAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            if (request is null) throw new ArgumentNullException(nameof(request), "请求不可为空");

            var actual_request_type = request.GetType();
            var actual_response_type = typeof(TResponse);

            var invoker = RequestCahce<TResponse>.Delegates.GetOrAdd(actual_request_type, type =>
            {
                // 获取方法信息
                var genericMethod = _requestResponseMethod.MakeGenericMethod(type, actual_response_type);

                // 构建
                var sender_param = Expression.Parameter(typeof(ISender), "sender");
                var request_param = Expression.Parameter(typeof(object), "request");
                var ct_param = Expression.Parameter(typeof(CancellationToken), "ct");

                // 转为实际请求类型
                var convertedRequest = Expression.Convert(request_param, type);
                // 构建委托
                var call = Expression.Call(sender_param, genericMethod, convertedRequest, ct_param);
                var lambda = Expression.Lambda<RequestCahce<TResponse>.Delegate>(call, sender_param, request_param, ct_param);
                return lambda.Compile();
            });

            return invoker.Invoke(sender, request, cancellationToken);
        }
    }

    private static class RequestCahce
    {
        public delegate ValueTask Delegate(ISender sender, object request, CancellationToken ct);
        public static readonly ConcurrentDictionary<Type, Delegate> Delegates = new();
    }
    private static class RequestCahce<TResponse>
    {
        public delegate ValueTask<TResponse> Delegate(ISender sender, object request, CancellationToken ct);
        public static readonly ConcurrentDictionary<Type, Delegate> Delegates = new();
    }
}