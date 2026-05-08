using Microsoft.CodeAnalysis;
using ThabeSoft.Mediator.SourceGenerator.Extensions;
using ThabeSoft.Mediator.SourceGenerator.Models;

namespace ThabeSoft.Mediator.SourceGenerator.Services.Parsers;

/// <summary>
/// 通知处理器
/// </summary>
public class NotifcationHandlerTypeParser : ITypeParser
{
    public const string IgnoreAttributeFullName = "ThabeSoft.Mediator.IgnoreHandlerAttribute";

    public bool TryParse(INamedTypeSymbol serviceTypeSymbol, INamedTypeSymbol implementationTypeSymbol, out ITypeInfo? info)
    {
        info = null;

        // 有忽略标签
        var ignore_att = implementationTypeSymbol.GetAttributeData(IgnoreAttributeFullName);
        if (ignore_att is not null) return false;

        // 通知处理器目前只有一个泛型参数
        var type_args = serviceTypeSymbol.TypeArguments;
        if (type_args.Length != 1) return false;

        // 构建
        var result = HandlerInfo.TryCreateNotification(serviceTypeSymbol, implementationTypeSymbol, out var handler_info);
        info = handler_info;

        return result;
    }
}