using ThabeSoft.Mediator.SourceGenerator.Extensions;
using ThabeSoft.Mediator.SourceGenerator.Models;

namespace ThabeSoft.Mediator.SourceGenerator.Services.Builders;


/// <summary>
/// 
/// </summary>
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
        public static void AddMediatorMiddlewares(this IServiceCollection services, Action<IDescriptorCollection>? optionAction = null)
        {
            var middleware_descriptors = new DescriptorCollection();

{{statements_code}}

            optionAction?.Invoke(middleware_descriptors);

            var service_descriptors = middleware_descriptors.BuildToServiceDescriptors();
            foreach (var service_descriptor in service_descriptors) services.TryAddEnumerable(service_descriptor);
        }
    }
""";
    }

    // 生成处理器注册代码
    private static string GenerateRegisterStatements(MiddlewareInfo middlewareInfo, HandlerInfo handlerInfo)
    {
        if (middlewareInfo.Kind == HandlerKind.RequestResponse && handlerInfo.Kind == HandlerKind.RequestResponse)
        {
            return $"""
            // {middlewareInfo.Kind}
            middleware_descriptors.AddRequestMiddleware<
                {middlewareInfo.ImplementationTypeSymbol.ToDisplayString(TypeParserExtensiosn.NonGenericFullNameFormat)}<
                    {handlerInfo.InputTypeSymbol},
                    {handlerInfo.OutputTypeSymbol}>,
                {handlerInfo.InputTypeSymbol},
                {handlerInfo.OutputTypeSymbol}>();
""";
        }
        if (middlewareInfo.Kind == HandlerKind.Request && handlerInfo.Kind == HandlerKind.Request)
        {
            return $"""
            // {middlewareInfo.Kind}
            middleware_descriptors.AddRequestMiddleware<
                {middlewareInfo.ImplementationTypeSymbol.ToDisplayString(TypeParserExtensiosn.NonGenericFullNameFormat)}<
                    {handlerInfo.InputTypeSymbol}>,
                {handlerInfo.InputTypeSymbol}>();
""";
        }

        return string.Empty;
    }
}
