using Microsoft.CodeAnalysis;

namespace ThabeSoft.Mediator.SourceGenerator.Extensions;

public static class TypeParserExtensiosn
{
    /// <summary>
    ///  业务显示格式  名字空间.类型 (不包含泛型, 如: ThabeSoft.Mediator.IRequesthandler
    /// </summary>
    public static SymbolDisplayFormat NonGenericFullNameFormat { get; } = new
    (
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
        genericsOptions: SymbolDisplayGenericsOptions.None
    );


    /// <summary>
    /// 获取特性信息
    /// </summary>
    /// <param name="classSymbol"></param>
    /// <param name="attributeFullName"></param>
    /// <returns></returns>
    public static AttributeData? GetAttributeData(this INamedTypeSymbol classSymbol, string attributeFullName)
    {
        var attributes = classSymbol.GetAttributes();

        foreach (var att in attributes)
        {
            var att_full_name = att.AttributeClass?.ToDisplayString();
            if (att_full_name == attributeFullName) return att;
        }

        return null;
    }
}