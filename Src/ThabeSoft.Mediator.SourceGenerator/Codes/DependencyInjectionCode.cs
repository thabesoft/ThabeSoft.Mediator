namespace ThabeSoft.Mediator.SourceGenerator.Codes;


internal static class DependencyInjectionCode
{
    public static string Namespace { get; } = "Microsoft.Extensions.DependencyInjection";

    public static string[] Usings { get; } =
    [
        "Microsoft.Extensions.DependencyInjection.Extensions",
        "ThabeSoft.Mediator",
        "ThabeSoft.Mediator.DependencyInjection"
    ];

    public static string FromHandlerInfos(List<HandlerInfo> handlers)
    {
        var event_handlers_register_code = string.Join("\n\n", handlers.Select(GenerateInjectionCode));

        string code = $$"""
{{GeneratorHelper.GenerateFileHead()}}

{{GeneratorHelper.GenerateUsingCode(Usings)}}


namespace {{Namespace}}
{
    internal static class ThabeSoftMediatorDependencyInjectionExtensions
    {
        public static void AddMediatorHandlers(this IServiceCollection services, Action<HandlerDescriptorCollection>? optionAction = null)
        {
            var handler_descriptors = new HandlerDescriptorCollection();

{{event_handlers_register_code}}

            optionAction?.Invoke(handler_descriptors);

            var service_descriptors = handler_descriptors.BuildToServiceDescriptors();
            foreach (var service_descriptor in service_descriptors) services.TryAddEnumerable(service_descriptor);
        }
    }
}
""";
        return code;
    }


    private static string GenerateInjectionCode(HandlerInfo info)
    {
        if (info.Kind == HandlerKind.Command && info.ReturnTypeFullName is null)
        {
            return $"""
            // {info.HandlerTypeFullName}
            handler_descriptors.AddCommand<{info.HandlerTypeFullName}, {info.MessageTypeFullName}>();
""";
        }
        if (info.Kind == HandlerKind.Command && info.ReturnTypeFullName is not null)
        {
            return $"""
            // {info.HandlerTypeFullName}
            handler_descriptors.AddCommand<{info.HandlerTypeFullName}, {info.MessageTypeFullName}, {info.ReturnTypeFullName}>();
""";
        }
        if (info.Kind == HandlerKind.Query && info.ReturnTypeFullName is not null)
        {
            return $"""
            // {info.HandlerTypeFullName}
            handler_descriptors.AddQuery<{info.HandlerTypeFullName}, {info.MessageTypeFullName}, {info.ReturnTypeFullName}>();
""";
        }

        return $"""
            // {info.HandlerTypeFullName}
            handler_descriptors.AddEvent<{info.HandlerTypeFullName}, {info.MessageTypeFullName}>();
""";
    }
}
