namespace ThabeSoft.Mediator;


/// <summary>
/// 请求
/// </summary>
public interface IRequest;

/// <summary>
/// 请求-响应
/// </summary>
/// <typeparam name="TResponse">响应</typeparam>
public interface IRequest<out TResponse>;