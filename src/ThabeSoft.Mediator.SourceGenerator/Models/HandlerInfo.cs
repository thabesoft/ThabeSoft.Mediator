using Microsoft.CodeAnalysis;
using ThabeSoft.Mediator.SourceGenerator.Extensions;

namespace ThabeSoft.Mediator.SourceGenerator.Models;

/// <summary>
/// 处理器信息
/// </summary>
public sealed record class HandlerInfo : TypeInfoBase
{
    public const string RequesServiceNonFullName = "ThabeSoft.Mediator.IRequestHandler";
    public const string NoficationServiceNonFullName = "ThabeSoft.Mediator.INoficationHandler";

    /// <summary>
    /// 类型
    /// </summary>
    public readonly HandlerKind Kind;

    /// <summary>
    /// 输入类型
    /// </summary>
    public INamedTypeSymbol InputTypeSymbol { get; }
    /// <summary>
    /// 输出类型
    /// </summary>
    public INamedTypeSymbol? OutputTypeSymbol { get; }


    private HandlerInfo(
        HandlerKind kind,
        INamedTypeSymbol serviceTypeSymbol,
        INamedTypeSymbol implementationTypeSymbol,
        INamedTypeSymbol requestTypeSymbol,
        INamedTypeSymbol? responseTypeSymbol
        ) : base(serviceTypeSymbol, implementationTypeSymbol)
    {
        Kind = kind;
        InputTypeSymbol = requestTypeSymbol;
        OutputTypeSymbol = responseTypeSymbol;
    }


    public static bool TryCreateRequest(
        INamedTypeSymbol serviceTypeSymbol,
        INamedTypeSymbol implementationTypeSymbol,
        out HandlerInfo? handlerInfo)
    {
        handlerInfo = default;

        // 请求参数必须是具体类型
        var type_args = serviceTypeSymbol.TypeArguments;
        if (type_args.Length != 1) return false;
        if (type_args[0] is not INamedTypeSymbol req_type_symbol) return false;

        var name = serviceTypeSymbol.ToDisplayString(TypeParserExtensiosn.GlobalNonGenericFullName);
        if (name != RequesServiceNonFullName) return false;

        handlerInfo = new HandlerInfo(
            HandlerKind.Request,
            serviceTypeSymbol,
            implementationTypeSymbol,
            req_type_symbol,
            null);
        return true;
    }
    public static bool TryCreateRequestResponse(
        INamedTypeSymbol serviceTypeSymbol,
        INamedTypeSymbol implementationTypeSymbol,
        out HandlerInfo? handlerInfo)
    {
        handlerInfo = default;

        var type_args = serviceTypeSymbol.TypeArguments;
        if (type_args.Length != 2) return false;

        if (type_args[0] is not INamedTypeSymbol req_type_symbol) return false;
        if (type_args[1] is not INamedTypeSymbol resp_type_symbol) return false;

        var name = serviceTypeSymbol.ToDisplayString(TypeParserExtensiosn.GlobalNonGenericFullName);
        if (name != RequesServiceNonFullName) return false;

        handlerInfo = new HandlerInfo(
            HandlerKind.RequestResponse,
            serviceTypeSymbol,
            implementationTypeSymbol,
            req_type_symbol,
            resp_type_symbol);
        return true;
    }
    public static bool TryCreateNotification(
        INamedTypeSymbol serviceTypeSymbol,
        INamedTypeSymbol implementationTypeSymbol,
        out HandlerInfo? handlerInfo)
    {
        handlerInfo = default;

        var type_args = serviceTypeSymbol.TypeArguments;
        if (type_args.Length != 1) return false;

        if (type_args[0] is not INamedTypeSymbol notify_type_symbol) return false;

        var name = serviceTypeSymbol.ToDisplayString(TypeParserExtensiosn.GlobalNonGenericFullName);
        if (name != NoficationServiceNonFullName) return false;

        handlerInfo = new HandlerInfo(
            HandlerKind.RequestResponse,
            serviceTypeSymbol,
            implementationTypeSymbol,
            notify_type_symbol,
            null);
        return true;
    }
}