using Microsoft.CodeAnalysis;
using ThabeSoft.Mediator.SourceGenerator.Models;

namespace ThabeSoft.Mediator.SourceGenerator.Services.Parsers;

/// <summary>
/// 中间件信息供者
/// </summary>
public interface ITypeParser
{
    bool TryParse(INamedTypeSymbol serviceTypeSymbol, INamedTypeSymbol implementationTypeSymbol, out ITypeInfo? info);
}
