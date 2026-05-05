namespace ThabeSoft.Mediator.Lifetime;


/// <summary>
/// 作用域, 在同一个作用域中是同一个实例
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class ScopedAttribute : Attribute { }
