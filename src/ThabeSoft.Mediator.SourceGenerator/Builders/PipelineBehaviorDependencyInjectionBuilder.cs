using Microsoft.CodeAnalysis;
using ThabeSoft.Mediator.SourceGenerator.Extensions;
using ThabeSoft.Mediator.SourceGenerator.Models;

namespace ThabeSoft.Mediator.SourceGenerator.Builders;


internal class PipelineBehaviorDependencyInjectionBuilder : CodeFileBuilderBase
{
    public PipelineBehaviorDependencyInjectionBuilder() : base(
        fileName: "MiddlewareDependencyInjection.g.cs",
        @namespace: "Microsoft.Extensions.DependencyInjection")
    {
        AddUsingNamespace("Microsoft.Extensions.DependencyInjection");
        AddUsingNamespace("Microsoft.Extensions.DependencyInjection.Extensions");
        AddUsingNamespace("ThabeSoft.Mediator");
        AddUsingNamespace("ThabeSoft.Mediator.DependencyInjection");
    }

    protected override string BuildContentStatements(IReadOnlyCollection<TypeRegistration> infos)
    {
        var handlers = infos.Where(x => x.Kind == TypeRegistrationKind.Handler).ToArray();
        var behaviors = infos.Where(x => x.Kind == TypeRegistrationKind.PipelineBehavior)
            .SelectMany(behavior => handlers
                .Select(handler => behavior
                    .CreateConcretePipelineBehavior(handler.InputTypeSymbol!, handler.OutputTypeSymbol!)
                )
            );

        var statements = behaviors
            .Distinct()
            .Select(GenerateRegisterStatements)
            .Where(x => !string.IsNullOrWhiteSpace(x));

        var statements_code = string.Join(NewLine, statements);
        if (string.IsNullOrWhiteSpace(statements_code)) return string.Empty;


        return $$"""
    internal static class ThabeSoftMiddlewareDependencyInjectionExtensions
    {
        public static void AddMediatorPipelineBehaviors(this IServiceCollection services, Action<IDescriptorCollection>? optionAction = null)
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
    private static string GenerateRegisterStatements(TypeRegistration info)
    {
        if (info.HandlerKind == HandlerKind.RequestResponse && info.InputTypeSymbol is not null && info.OutputTypeSymbol is not null)
        {
            return $"""
                // {info.Kind}
                x.AddRequestBehavior<
                    {info.ImplementationTypeSymbol.ToDisplayString(TypeParserExtensiosn.GlobalNonGenericFullName)}<
                        {info.InputTypeSymbol.ToDisplayString(TypeParserExtensiosn.GlobalFullName)},
                        {info.OutputTypeSymbol.ToDisplayString(TypeParserExtensiosn.GlobalFullName)}>,
                    {info.InputTypeSymbol.ToDisplayString(TypeParserExtensiosn.GlobalFullName)},
                    {info.OutputTypeSymbol.ToDisplayString(TypeParserExtensiosn.GlobalFullName)}>();
""";
        }
        if (info.HandlerKind == HandlerKind.Request && info.InputTypeSymbol is not null)
        {
            return $"""
                // {info.Kind}
                x.AddRequestBehavior<
                    {info.ImplementationTypeSymbol.ToDisplayString(TypeParserExtensiosn.GlobalNonGenericFullName)}<
                        {info.InputTypeSymbol.ToDisplayString(TypeParserExtensiosn.GlobalFullName)}>,
                    {info.InputTypeSymbol.ToDisplayString(TypeParserExtensiosn.GlobalFullName)}>();
""";
        }
        if (info.HandlerKind == HandlerKind.Notification && info.InputTypeSymbol is not null)
        {
            return $"""
                // {info.Kind}
                x.AddNotificationBehavior<
                    {info.ImplementationTypeSymbol.ToDisplayString(TypeParserExtensiosn.GlobalNonGenericFullName)}<
                        {info.InputTypeSymbol.ToDisplayString(TypeParserExtensiosn.GlobalFullName)}>,
                    {info.InputTypeSymbol.ToDisplayString(TypeParserExtensiosn.GlobalFullName)}>();
""";
        }

        return string.Empty;
    }
}
