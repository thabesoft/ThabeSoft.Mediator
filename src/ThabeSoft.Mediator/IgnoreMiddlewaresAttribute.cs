namespace ThabeSoft.Mediator;

/// <summary>
/// 忽略中间件
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IgnoreMiddlewareAttribute : Attribute;