using Microsoft.CodeAnalysis;
using ThabeSoft.Mediator.SourceGenerator.Extensions;

namespace ThabeSoft.Mediator.SourceGenerator.Models;


/// <summary>
/// 类型注册信息
/// </summary>
public record class TypeRegistration
{
    #region --工厂方法--

    public const string RequestHandlerServiceNonFullName = "global::ThabeSoft.Mediator.IRequestHandler";
    public const string NoficationServiceNonFullName = "global::ThabeSoft.Mediator.INoficationHandler";
    public const string RequestPipelineBehaviorServiceNonGenericFullName = "global::ThabeSoft.Mediator.IRequestPipelineBehavior";
    public const string NotificationPipelineBehaviorServiceNonGenericFullName = "global::ThabeSoft.Mediator.INotificationPipelineBehavior";
    public const string IgnoreAttributeFullName = "global::ThabeSoft.Mediator.IgnoreAttribute";

    /// <summary>
    /// 尝试创建信息
    /// </summary>
    /// <param name="serviceTypeSymbol">业务类型</param>
    /// <param name="implementationTypeSymbol">实现类型</param>
    public static bool TryCreate(INamedTypeSymbol serviceTypeSymbol, INamedTypeSymbol implementationTypeSymbol, out TypeRegistration? info)
    {
        info = null;

        // 有忽略标签
        var ignore_att = implementationTypeSymbol.GetAttributeData(IgnoreAttributeFullName);
        if (ignore_att is not null) return false;

        // 泛型小于一个, 没有泛型肯定不是 <T>
        var type_args = serviceTypeSymbol.TypeArguments;
        if (type_args.Length < 1) return false;


        var service_full_name = serviceTypeSymbol.ToDisplayString(TypeParserExtensiosn.GlobalNonGenericFullName);

        // IRequestHandler<TRequest>
        if (service_full_name == RequestHandlerServiceNonFullName && type_args.Length == 1)
        {
            return TryCreateRequest(RequestHandlerServiceNonFullName, TypeRegistrationKind.Handler, false, serviceTypeSymbol, implementationTypeSymbol, out info);
        }
        // IRequestHandler<TRequest, TResponse>
        if (service_full_name == RequestHandlerServiceNonFullName && type_args.Length == 2)
        {
            return TryCreateRequestResponse(RequestHandlerServiceNonFullName, TypeRegistrationKind.Handler, false, serviceTypeSymbol, implementationTypeSymbol, out info);
        }
        // INoficationHandler<TNofication>
        if (service_full_name == NoficationServiceNonFullName && type_args.Length == 1)
        {
            return TryCreateNotification(NoficationServiceNonFullName, TypeRegistrationKind.Handler, false, serviceTypeSymbol, implementationTypeSymbol, out info);
        }

        // IRequestPipelineBehavior<TRequest>
        if (service_full_name == RequestPipelineBehaviorServiceNonGenericFullName && type_args.Length == 1)
        {
            return TryCreateRequest(RequestPipelineBehaviorServiceNonGenericFullName, TypeRegistrationKind.PipelineBehavior, true, serviceTypeSymbol, implementationTypeSymbol, out info);
        }
        // IRequestPipelineBehavior<TRequest, TResponse>
        if (service_full_name == RequestPipelineBehaviorServiceNonGenericFullName && type_args.Length == 2)
        {
            return TryCreateRequestResponse(RequestPipelineBehaviorServiceNonGenericFullName, TypeRegistrationKind.PipelineBehavior, true, serviceTypeSymbol, implementationTypeSymbol, out info);
        }
        // INotificationPipelineBehavior<TNofication>
        if (service_full_name == NotificationPipelineBehaviorServiceNonGenericFullName && type_args.Length == 1)
        {
            return TryCreateNotification(NotificationPipelineBehaviorServiceNonGenericFullName, TypeRegistrationKind.PipelineBehavior, true, serviceTypeSymbol, implementationTypeSymbol, out info);
        }

        return false;
    }

    #endregion

    /// <summary>
    /// 种类
    /// </summary>
    public TypeRegistrationKind Kind { get; }

    /// <summary>
    /// 处理器类型
    /// </summary>
    public HandlerKind HandlerKind { get; }

    /// <summary>
    /// 接口名称
    /// </summary>
    public INamedTypeSymbol ServiceTypeSymbol { get; }

    /// <summary>
    /// 实现类型名称
    /// </summary>
    public INamedTypeSymbol ImplementationTypeSymbol { get; }

    /// <summary>
    /// 输入类型
    /// </summary>
    public INamedTypeSymbol? InputTypeSymbol { get; }
    /// <summary>
    /// 输出类型
    /// </summary>
    public INamedTypeSymbol? OutputTypeSymbol { get; }


    private TypeRegistration(
        TypeRegistrationKind kind,
        HandlerKind handlerKind,
        INamedTypeSymbol serviceTypeSymbol,
        INamedTypeSymbol implementationTypeSymbol,
        INamedTypeSymbol? inputTypeSymbol,
        INamedTypeSymbol? outputTypeSymbol
        )
    {
        Kind = kind;
        HandlerKind = handlerKind;
        ServiceTypeSymbol = serviceTypeSymbol;
        ImplementationTypeSymbol = implementationTypeSymbol;
        InputTypeSymbol = inputTypeSymbol;
        OutputTypeSymbol = outputTypeSymbol;
    }


    /// <summary>
    /// 创建具体管道行为类型
    /// </summary>
    /// <param name="inputTypeSymbol">输入类型</param>
    /// <param name="outputTypeSymbol">输出类型</param>
    /// <exception cref="InvalidOperationException">此类必须是<see cref="TypeRegistrationKind.PipelineBehavior"/>否则抛出</exception>
    public TypeRegistration CreateConcretePipelineBehavior(INamedTypeSymbol inputTypeSymbol, INamedTypeSymbol outputTypeSymbol)
    {
        if (Kind != TypeRegistrationKind.PipelineBehavior) throw new InvalidOperationException("非管道行为无法创建");
        return new TypeRegistration(Kind, HandlerKind, ServiceTypeSymbol, ImplementationTypeSymbol, inputTypeSymbol, outputTypeSymbol);
    }

    private static bool TryCreateRequest(
        string serviceNonGenericFullName,
        TypeRegistrationKind typeKind,
        bool generic,
        INamedTypeSymbol serviceTypeSymbol,
        INamedTypeSymbol implementationTypeSymbol,
        out TypeRegistration? handlerInfo)
    {
        handlerInfo = default;

        // 接口参数数量验证
        var type_args = serviceTypeSymbol.TypeArguments;
        if (type_args.Length != 1) return false;

        INamedTypeSymbol? req_type_symbol = null;

        // 泛型验证
        if (generic && type_args[0] is not ITypeParameterSymbol) return false;
        if (!generic && type_args[0] is INamedTypeSymbol req) req_type_symbol = req;

        // 接口名称验证
        var name = serviceTypeSymbol.ToDisplayString(TypeParserExtensiosn.GlobalNonGenericFullName);
        if (name != serviceNonGenericFullName) return false;

        handlerInfo = new TypeRegistration
        (
            kind: typeKind,
            handlerKind: HandlerKind.Request,
            serviceTypeSymbol: serviceTypeSymbol,
            implementationTypeSymbol: implementationTypeSymbol,
            inputTypeSymbol: req_type_symbol,
            outputTypeSymbol: null
        );
        return true;
    }
    private static bool TryCreateRequestResponse(
        string serviceNonGenericFullName,
        TypeRegistrationKind typeKind,
        bool generic,
        INamedTypeSymbol serviceTypeSymbol,
        INamedTypeSymbol implementationTypeSymbol,
        out TypeRegistration? handlerInfo)
    {
        handlerInfo = default;

        // 接口参数数量验证
        var type_args = serviceTypeSymbol.TypeArguments;
        if (type_args.Length != 2) return false;

        INamedTypeSymbol? req_type_symbol = null;
        INamedTypeSymbol? resp_type_symbol = null;

        // 泛型验证
        if (generic && (type_args[0] is not ITypeParameterSymbol || type_args[0] is not ITypeParameterSymbol)) return false;
        if (!generic && type_args[0] is INamedTypeSymbol req) req_type_symbol = req;
        if (!generic && type_args[1] is INamedTypeSymbol resp) resp_type_symbol = resp;

        // 接口名称验证
        var name = serviceTypeSymbol.ToDisplayString(TypeParserExtensiosn.GlobalNonGenericFullName);
        if (name != serviceNonGenericFullName) return false;

        handlerInfo = new TypeRegistration
        (
            kind: typeKind,
            handlerKind: HandlerKind.RequestResponse,
            serviceTypeSymbol: serviceTypeSymbol,
            implementationTypeSymbol: implementationTypeSymbol,
            inputTypeSymbol: req_type_symbol,
            outputTypeSymbol: resp_type_symbol
        );
        return true;
    }
    private static bool TryCreateNotification(
        string serviceNonGenericFullName,
        TypeRegistrationKind typeKind,
        bool generic,
        INamedTypeSymbol serviceTypeSymbol,
        INamedTypeSymbol implementationTypeSymbol,
        out TypeRegistration? handlerInfo)
    {
        handlerInfo = default;

        // 接口参数数量验证
        var type_args = serviceTypeSymbol.TypeArguments;
        if (type_args.Length != 1) return false;

        INamedTypeSymbol? notify_type_symbol = null;

        // 泛型验证
        if (generic && type_args[0] is not ITypeParameterSymbol) return false;
        if (!generic && type_args[0] is INamedTypeSymbol notify) notify_type_symbol = notify;

        // 接口名称验证
        var name = serviceTypeSymbol.ToDisplayString(TypeParserExtensiosn.GlobalNonGenericFullName);
        if (name != serviceNonGenericFullName) return false;

        handlerInfo = new TypeRegistration
        (
            kind: typeKind,
            handlerKind: HandlerKind.Notification,
            serviceTypeSymbol: serviceTypeSymbol,
            implementationTypeSymbol: implementationTypeSymbol,
            inputTypeSymbol: notify_type_symbol,
            outputTypeSymbol: null
        );
        return true;
    }
}