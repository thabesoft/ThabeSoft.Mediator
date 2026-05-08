using Microsoft.CodeAnalysis;

namespace ThabeSoft.Mediator.SourceGenerator.Models;


/// <summary>
/// 类型信息
/// </summary>
public interface ITypeInfo
{
    /// <summary>
    /// 接口名称
    /// </summary>
    INamedTypeSymbol ServiceTypeSymbol { get; }
    /// <summary>
    /// 实现类型名称
    /// </summary>
    INamedTypeSymbol ImplementationTypeSymbol { get; }
}


public abstract record TypeInfoBase : ITypeInfo
{
    public INamedTypeSymbol ServiceTypeSymbol { get; }
    public INamedTypeSymbol ImplementationTypeSymbol { get; }


    protected TypeInfoBase(
        INamedTypeSymbol serviceTypeSymbol,
        INamedTypeSymbol implementationTypeSymbol
        )
    {
        ServiceTypeSymbol = serviceTypeSymbol;
        ImplementationTypeSymbol = implementationTypeSymbol;
    }
}