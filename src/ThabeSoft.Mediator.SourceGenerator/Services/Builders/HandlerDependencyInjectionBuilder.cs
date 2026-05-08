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
        if (_handlers.Length <= 0) return string.Empty;

        // 处理器
        var statements = _handlers.Select(GenerateRegisterStatements).Where(x => !string.IsNullOrEmpty(x));
        var statements_code = string.Join($"{NewLine}{NewLine}", statements);

        return $$"""
    internal static class ThabeSoftMediatorHandlerDependencyInjectionExtensions
    {
        public static void AddMediatorHandlers(this IServiceCollection services, Action<IHandlerDescriptorCollection>? optionAction = null)
        {
            var handler_descriptors = new HandlerDescriptorCollection();

{{statements_code}}

            optionAction?.Invoke(handler_descriptors);

            var service_descriptors = handler_descriptors.BuildToServiceDescriptors();
            foreach (var service_descriptor in service_descriptors) services.TryAddEnumerable(service_descriptor);
        }
    }
""";
    }

    // 生成处理器注册代码
    private static string GenerateRegisterStatements(HandlerInfo info)
    {
        if (info.Kind == HandlerKind.RequestResponse)
        {
            return $"""
            // {info.ImplementationTypeSymbol}
            handler_descriptors.AddRequest<
                {info.ImplementationTypeSymbol},
                {info.InputTypeSymbol},
                {info.OutputTypeSymbol}>();
""";
        }
        if (info.Kind == HandlerKind.Request)
        {
            return $"""
            // {info.ImplementationTypeSymbol}
            handler_descriptors.AddRequest<
                {info.ImplementationTypeSymbol},
                {info.InputTypeSymbol}>();
""";
        }
        if (info.Kind == HandlerKind.Notification)
        {
            return $"""
            // {info.ImplementationTypeSymbol}
            handler_descriptors.AddNotification<
                {info.ImplementationTypeSymbol},
                {info.InputTypeSymbol}>();
""";
        }

        return string.Empty;
    }
}
