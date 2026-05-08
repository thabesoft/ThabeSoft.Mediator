using Microsoft.CodeAnalysis;
using ThabeSoft.Mediator.SourceGenerator.Extensions;
using ThabeSoft.Mediator.SourceGenerator.Models;

namespace ThabeSoft.Mediator.SourceGenerator.Services.Parsers;


/// <summary>
/// 出处理器类型解析器
/// </summary>
/// <param name="register"></param>
public class RequestHandlerTypeParser : ITypeParser
{
    public const string IgnoreAttributeFullName = "ThabeSoft.Mediator.IgnoreHandlerAttribute";

    public bool TryParse(INamedTypeSymbol serviceTypeSymbol, INamedTypeSymbol implementationTypeSymbol, out ITypeInfo? info)
    {
        info = null;

        // 有忽略标签
        var ignore_att = implementationTypeSymbol.GetAttributeData(IgnoreAttributeFullName);
        if (ignore_att is not null) return false;

        // 泛型小于一个, 没有泛型肯定不是 <T>
        var type_args = serviceTypeSymbol.TypeArguments;
        if (type_args.Length < 1) return false;

        // 无响应请求
        if (type_args.Length == 1)
        {
            var result = HandlerInfo.TryCreateRequest(serviceTypeSymbol, implementationTypeSymbol, out var handler_info);
            info = handler_info;
            return result;
        }

        // 请求响应
        if (type_args.Length == 2)
        {
            var result = HandlerInfo.TryCreateRequestResponse(serviceTypeSymbol, implementationTypeSymbol, out var handler_info);
            info = handler_info;
            return result;
        }

        return false;
    }
}