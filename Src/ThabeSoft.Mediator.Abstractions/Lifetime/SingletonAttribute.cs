namespace ThabeSoft.Mediator.Lifetime;


/// <summary>
/// 单例, 和容器相同的生命周期
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class SingletonAttribute : Attribute { }
