namespace ThabeSoft.Mediator;


/// <summary>
/// 忽略处理器
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IgnoreHandlerAttribute : Attribute;