using Microsoft.CodeAnalysis;
using ThabeSoft.Mediator.SourceGenerator.Extensions;

namespace ThabeSoft.Mediator.SourceGenerator.Models;


/// <summary>
/// 中间件信息
/// </summary>
public sealed record class MiddlewareInfo : TypeInfoBase
{
    public const string ServiceNonGenericFullName = "ThabeSoft.Mediator.IMiddleware";

    /// <summary>
    /// 种类
    /// </summary>
    public HandlerKind Kind { get; }

    private MiddlewareInfo(
        HandlerKind kind,
        INamedTypeSymbol serviceTypeSymbol,
        INamedTypeSymbol implementationTypeSymbol
        ) : base(serviceTypeSymbol, implementationTypeSymbol)
    {
        Kind = kind;
    }


    // 创建请求中间件
    public static bool TryCreateRequest(
        INamedTypeSymbol serviceTypeFullName,
        INamedTypeSymbol implementationTypeFullName,
        out MiddlewareInfo? handlerInfo)
    {
        handlerInfo = default;

        // IMiddlware<TRequest>
        var type_args = serviceTypeFullName.TypeArguments;
        if (type_args.Length != 1) return false;
        if (type_args[0] is not ITypeParameterSymbol) return false;

        var name = serviceTypeFullName.ToDisplayString(TypeParserExtensiosn.NonGenericFullNameFormat);
        if (name != ServiceNonGenericFullName) return false;

        handlerInfo = new MiddlewareInfo(
            HandlerKind.Request,
            serviceTypeFullName,
            implementationTypeFullName);

        return true;
    }

    // 创建请求响应中间件
    public static bool TryCreateRequestResponse(
        INamedTypeSymbol serviceTypeFullName,
        INamedTypeSymbol implementationTypeFullName,
        out MiddlewareInfo? handlerInfo)
    {
        handlerInfo = default;

        // IMiddlware<TRequest>
        var type_args = serviceTypeFullName.TypeArguments;
        if (type_args.Length != 2) return false;

        if (type_args[0] is not ITypeParameterSymbol) return false;
        if (type_args[0] is not ITypeParameterSymbol) return false;

        var name = serviceTypeFullName.ToDisplayString(TypeParserExtensiosn.NonGenericFullNameFormat);
        if (name != ServiceNonGenericFullName) return false;

        handlerInfo = new MiddlewareInfo(
            HandlerKind.RequestResponse,
            serviceTypeFullName,
            implementationTypeFullName);

        return true;
    }
}