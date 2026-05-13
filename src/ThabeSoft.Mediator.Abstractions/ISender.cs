using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace ThabeSoft.Mediator;

/// <summary>
/// 发送器
/// </summary>
public interface ISender
{
    ValueTask SendAsync<TRequest>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : IRequest;
    ValueTask<TResponse> SendAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : IRequest<TResponse>;
}
