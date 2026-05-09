using Microsoft.CodeAnalysis;
using ThabeSoft.Mediator.SourceGenerator.Extensions;
using ThabeSoft.Mediator.SourceGenerator.Models;

namespace ThabeSoft.Mediator.SourceGenerator.Services.Builders;


internal class MiddlewareDependencyInjectionBuilder : CodeFileBuilderBase
{
    public MiddlewareDependencyInjectionBuilder() : base(
        fileName: "MiddlewareDependencyInjection.g.cs",
        @namespace: "Microsoft.Extensions.DependencyInjection")
    {
        AddUsingNamespace("Microsoft.Extensions.DependencyInjection");
        AddUsingNamespace("Microsoft.Extensions.DependencyInjection.Extensions");
        AddUsingNamespace("ThabeSoft.Mediator");
        AddUsingNamespace("ThabeSoft.Mediator.DependencyInjection");
    }

    protected override string BuildContentStatements(IReadOnlyCollection<ITypeInfo> typeTnfos)
    {
        var middlewares = typeTnfos.OfType<MiddlewareInfo>().ToArray();
        var handlers = typeTnfos.OfType<HandlerInfo>().ToArray();

        var statements = middlewares.SelectMany(m => handlers.Select(h => (middleware: m, handler: h)))
            .Select(x => GenerateRegisterStatements(x.middleware, x.handler))
            .Where(x => !string.IsNullOrWhiteSpace(x));
        var statements_code = string.Join(NewLine, statements);

        return $$"""
    internal static class ThabeSoftMiddlewareDependencyInjectionExtensions
    {
        public static void AddMediatorBehaviors(this IServiceCollection services, Action<IDescriptorCollection>? optionAction = null)
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
    private static string GenerateRegisterStatements(MiddlewareInfo middlewareInfo, HandlerInfo handlerInfo)
    {
        if (middlewareInfo.Kind == HandlerKind.RequestResponse && handlerInfo.OutputTypeSymbol is not null)
        {
            return $"""
                // {middlewareInfo.Kind}
                x.AddRequestBehavior<
                    {middlewareInfo.ImplementationTypeSymbol.ToDisplayString(TypeParserExtensiosn.GlobalNonGenericFullName)}<
                        {handlerInfo.InputTypeSymbol.ToDisplayString(TypeParserExtensiosn.GlobalFullName)},
                        {handlerInfo.OutputTypeSymbol.ToDisplayString(TypeParserExtensiosn.GlobalFullName)}>,
                    {handlerInfo.InputTypeSymbol.ToDisplayString(TypeParserExtensiosn.GlobalFullName)},
                    {handlerInfo.OutputTypeSymbol.ToDisplayString(TypeParserExtensiosn.GlobalFullName)}>();
""";
        }
        if (middlewareInfo.Kind == HandlerKind.Request)
        {
            return $"""
                // {middlewareInfo.Kind}
                x.AddRequestBehavior<
                    {middlewareInfo.ImplementationTypeSymbol.ToDisplayString(TypeParserExtensiosn.GlobalNonGenericFullName)}<
                        {handlerInfo.InputTypeSymbol.ToDisplayString(TypeParserExtensiosn.GlobalFullName)}>,
                    {handlerInfo.InputTypeSymbol.ToDisplayString(TypeParserExtensiosn.GlobalFullName)}>();
""";
        }
        if (middlewareInfo.Kind == HandlerKind.Notification)
        {
            return $"""
                // {middlewareInfo.Kind}
                x.AddNotificationBehavior<
                    {middlewareInfo.ImplementationTypeSymbol.ToDisplayString(TypeParserExtensiosn.GlobalNonGenericFullName)}<
                        {handlerInfo.InputTypeSymbol.ToDisplayString(TypeParserExtensiosn.GlobalFullName)}>,
                    {handlerInfo.InputTypeSymbol.ToDisplayString(TypeParserExtensiosn.GlobalFullName)}>();
""";
        }

        return string.Empty;
    }
}
