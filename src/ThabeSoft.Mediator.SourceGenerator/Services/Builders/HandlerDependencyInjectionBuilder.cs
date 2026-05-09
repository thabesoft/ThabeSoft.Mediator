using ThabeSoft.Mediator.SourceGenerator.Extensions;
using ThabeSoft.Mediator.SourceGenerator.Models;

namespace ThabeSoft.Mediator.SourceGenerator.Services.Builders;


/// <summary>
/// 处理器依赖注入
/// </summary>
public sealed class HandlerDependencyInjectionBuilder : CodeFileBuilderBase
{
    public HandlerDependencyInjectionBuilder() : base(
        fileName: "HandlerDependencyInjection.g.cs",
        @namespace:"Microsoft.Extensions.DependencyInjection")
    {
        AddUsingNamespace("Microsoft.Extensions.DependencyInjection.Extensions");
        AddUsingNamespace("ThabeSoft.Mediator");
        AddUsingNamespace("ThabeSoft.Mediator.DependencyInjection");
    }

    protected override string BuildContentStatements(IReadOnlyCollection<ITypeInfo> typeTnfos)
    {
        var _handlers = typeTnfos.OfType<HandlerInfo>().ToArray();
        if (_handlers.Length == 0) return string.Empty;

        // 处理器
        var statements = _handlers.Select(GenerateRegisterStatements).Where(x => !string.IsNullOrEmpty(x));
        var statements_code = string.Join(NewLine + NewLine, statements);

        return $$"""
    internal static class ThabeSoftMediatorHandlerDependencyInjectionExtensions
    {
        public static void AddMediatorHandlers(this IServiceCollection services, Action<IDescriptorCollection>? optionAction = null)
        {
            services.ConfigureMediator(x =>
            {
{{statements_code}}
            });
        }
    }
""";
    }

    // 生成处理器注册代码
    private static string GenerateRegisterStatements(HandlerInfo info)
    {
        if (info.Kind == HandlerKind.RequestResponse && info.OutputTypeSymbol is not null)
        {
            return $"""
                // {info.ImplementationTypeSymbol}
                x.AddRequestHandler<
                    {info.ImplementationTypeSymbol.ToDisplayString(TypeParserExtensiosn.GlobalFullName)},
                    {info.InputTypeSymbol.ToDisplayString(TypeParserExtensiosn.GlobalFullName)},
                    {info.OutputTypeSymbol.ToDisplayString(TypeParserExtensiosn.GlobalFullName)}>();
""";
        }
        if (info.Kind == HandlerKind.Request)
        {
            return $"""
                // {info.ImplementationTypeSymbol}
                x.AddRequestHandler<
                    {info.ImplementationTypeSymbol.ToDisplayString(TypeParserExtensiosn.GlobalFullName)},
                    {info.InputTypeSymbol.ToDisplayString(TypeParserExtensiosn.GlobalFullName)}>();
""";
        }
        if (info.Kind == HandlerKind.Notification)
        {
            return $"""
                // {info.ImplementationTypeSymbol}
                x.AddNotificationHandler<
                    {info.ImplementationTypeSymbol.ToDisplayString(TypeParserExtensiosn.GlobalFullName)},
                    {info.InputTypeSymbol.ToDisplayString(TypeParserExtensiosn.GlobalFullName)}>();
""";
        }

        return string.Empty;
    }
}
