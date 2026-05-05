using System;

namespace ThabeSoft.Mediator.Lifetime;


/// <summary>
/// 瞬态, 每次获取都是新实例
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class TransientAttribute : Attribute { }
