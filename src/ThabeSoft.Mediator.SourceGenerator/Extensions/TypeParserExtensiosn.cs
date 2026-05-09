using Microsoft.CodeAnalysis;

namespace ThabeSoft.Mediator.SourceGenerator.Extensions;

public static class TypeParserExtensiosn
{
    /// <summary>
    /// 不包含泛型的全名称, 如 global::ThabeSoft.Mediator.IRequesthandler
    /// </summary>
    public static SymbolDisplayFormat GlobalNonGenericFullName { get; } = new
    (
        globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Included,
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
        genericsOptions: SymbolDisplayGenericsOptions.None
    );

    /// <summary>
    /// 全名, 如 global::ThabeSoft.Mediator.SourceGenerator.Extensions.TypeParserExtensiosn
    /// </summary>
    public static SymbolDisplayFormat GlobalFullName { get; } = new
    (
        globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Included,
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces
    );



    /// <summary>
    /// 获取特性信息
    /// </summary>
    /// <param name="classSymbol"></param>
    /// <param name="attributeFullName"></param>
    /// <returns></returns>
    public static AttributeData? GetAttributeData(this INamedTypeSymbol classSymbol, string attributeFullName)
    {
        foreach (var att in classSymbol.GetAttributes())
        {
            var att_full_name = att.AttributeClass?.ToDisplayString();
            if (att_full_name == attributeFullName) return att;
        }

        return null;
    }
}