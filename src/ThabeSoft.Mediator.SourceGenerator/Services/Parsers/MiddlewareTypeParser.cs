using Microsoft.CodeAnalysis;
using ThabeSoft.Mediator.SourceGenerator.Extensions;
using ThabeSoft.Mediator.SourceGenerator.Models;

namespace ThabeSoft.Mediator.SourceGenerator.Services.Parsers;


public class MiddlewareTypeParser : ITypeParser
{
    public const string IgnoreAttributeFullName = "ThabeSoft.Mediator.IgnoreMiddlewareAttribute";

    public bool TryParse(INamedTypeSymbol serviceTypeSymbol, INamedTypeSymbol implementationTypeSymbol, out ITypeInfo? info)
    {
        info = default;

        // 有忽略标签
        var ignore_att = implementationTypeSymbol.GetAttributeData(IgnoreAttributeFullName);
        if (ignore_att is not null) return false;

        var type_args = serviceTypeSymbol.TypeArguments;

        // 无响应请求
        if (type_args.Length == 1)
        {
            var result = MiddlewareInfo.TryCreateRequest(serviceTypeSymbol, implementationTypeSymbol, out var handler_info);
            info = handler_info;
            return result;
        }
        // 请求响应
        if (type_args.Length == 2)
        {
            var result = MiddlewareInfo.TryCreateRequestResponse(serviceTypeSymbol, implementationTypeSymbol, out var handler_info);
            info = handler_info;
            return result;
        }

        return false;
    }
}
