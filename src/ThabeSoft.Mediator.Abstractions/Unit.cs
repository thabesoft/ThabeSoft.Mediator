namespace ThabeSoft.Mediator;

/// <summary>
/// 空单元
/// </summary>
public sealed class Unit : IEquatable<Unit>
{
    public static readonly Unit Value = new();

    private Unit() { }
    public override string ToString() => "()";
    public bool Equals(Unit other) => true;
    public override bool Equals(object? obj) => obj is Unit;
    public override int GetHashCode() => 0;
}