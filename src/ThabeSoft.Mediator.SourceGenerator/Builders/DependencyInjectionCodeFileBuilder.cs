using ThabeSoft.Mediator.SourceGenerator.Models;

namespace ThabeSoft.Mediator.SourceGenerator.Builders;


/// <summary>
/// 
/// </summary>
internal class DependencyInjectionCodeFileBuilder : CodeFileBuilderBase
{
    private readonly List<HandlerInfo> _handlers;

    public DependencyInjectionCodeFileBuilder(List<HandlerInfo> handlers) : base("Microsoft.Extensions.DependencyInjection")
    {
        _handlers = handlers;

        AddUsingNamespace("Microsoft.Extensions.DependencyInjection.Extensions");
        AddUsingNamespace("ThabeSoft.Mediator");
        AddUsingNamespace("ThabeSoft.Mediator.DependencyInjection");
    }

    protected override string BuildContentStatements()
    {
        var event_handlers_register_code = string.Join(NewLine, _handlers.Select(GenerateHandlerRegisterStatements));

        return $$"""
    internal static class ThabeSoftMediatorDependencyInjectionExtensions
    {
        public static void AddMediatorHandlers(this IServiceCollection services, Action<IHandlerDescriptorCollection>? optionAction = null)
        {
            var handler_descriptors = new HandlerDescriptorCollection();

{{event_handlers_register_code}}

            optionAction?.Invoke(handler_descriptors);

            var service_descriptors = handler_descriptors.BuildToServiceDescriptors();
            foreach (var service_descriptor in service_descriptors) services.TryAddEnumerable(service_descriptor);
        }
    }
""";
    }

    // 生成处理器注册代码
    private static string GenerateHandlerRegisterStatements(HandlerInfo info)
    {
        if (info.Kind == HandlerKind.Request)
        {
            return $"""
            // {info.HandlerTypeFullName}
            handler_descriptors.AddRequest<{info.HandlerTypeFullName}, {info.InputTypeFullName}>();
""";
        }
        if (info.Kind == HandlerKind.RequestResponse)
        {
            return $"""
            // {info.HandlerTypeFullName}
            handler_descriptors.AddRequest<{info.HandlerTypeFullName}, {info.InputTypeFullName}, {info.OutputTypeFullName}>();
""";
        }
        if (info.Kind == HandlerKind.Notification)
        {
            return $"""
            // {info.HandlerTypeFullName}
            handler_descriptors.AddNotification<{info.HandlerTypeFullName}, {info.InputTypeFullName}>();
""";
        }

        return string.Empty;
    }
}
