using ThabeSoft.Mediator.SourceGenerator.Extensions;
using ThabeSoft.Mediator.SourceGenerator.Models;

namespace ThabeSoft.Mediator.SourceGenerator.Builders;


/// <summary>
/// 处理器依赖注入
/// </summary>
public sealed class DependencyInjectionBuilder : CodeFileBuilderBase
{
    public DependencyInjectionBuilder() : base(
        fileName: "DependencyInjection.g.cs",
        @namespace:"Microsoft.Extensions.DependencyInjection")
    {
        AddUsingNamespace("Microsoft.Extensions.DependencyInjection.Extensions");
        AddUsingNamespace("ThabeSoft.Mediator");
        AddUsingNamespace("ThabeSoft.Mediator.DependencyInjection");
    }

    protected override string BuildContentStatements(IReadOnlyCollection<TypeRegistration> infos)
    {
        var statements = infos
            .Where(x => x.Kind == TypeRegistrationKind.Handler)
            .Distinct()
            .Select(GenerateRegisterStatements)
            .Where(x => !string.IsNullOrEmpty(x));

        var statements_code = string.Join(NewLine + NewLine, statements);
        if (string.IsNullOrWhiteSpace(statements_code)) return string.Empty;


        return $$"""
    internal static partial class ThabeSoftMediatorDependencyInjectionExtensions
    {
        public static void AddGeneratedMediator(this IServiceCollection services, Action<IDescriptorCollection>? optionAction = null)
        {
            services.AddMediatorHandlers(optionAction);
            services.AddMediatorPipelineBehaviors(optionAction);
        }
    }
""";
    }

    // 生成处理器注册代码
    private static string GenerateRegisterStatements(TypeRegistration info)
    {
        if (info.HandlerKind == HandlerKind.RequestResponse && info.InputTypeSymbol is not null && info.OutputTypeSymbol is not null)
        {
            return $"""
                // {info.ImplementationTypeSymbol}
                x.AddRequestHandler<
                    {info.ImplementationTypeSymbol.ToDisplayString(TypeParserExtensiosn.GlobalFullName)},
                    {info.InputTypeSymbol.ToDisplayString(TypeParserExtensiosn.GlobalFullName)},
                    {info.OutputTypeSymbol.ToDisplayString(TypeParserExtensiosn.GlobalFullName)}>();
""";
        }
        if (info.HandlerKind == HandlerKind.Request && info.InputTypeSymbol is not null)
        {
            return $"""
                // {info.ImplementationTypeSymbol}
                x.AddRequestHandler<
                    {info.ImplementationTypeSymbol.ToDisplayString(TypeParserExtensiosn.GlobalFullName)},
                    {info.InputTypeSymbol.ToDisplayString(TypeParserExtensiosn.GlobalFullName)}>();
""";
        }
        if (info.HandlerKind == HandlerKind.Notification && info.InputTypeSymbol is not null)
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
