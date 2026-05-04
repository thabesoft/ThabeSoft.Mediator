namespace ThabeSoft.Mediator.SourceGenerator.Codes;


internal static class DependencyInjectionCode
{
    public static string Namespace { get; } = "Microsoft.Extensions.DependencyInjection";

    public static string[] Usings { get; } =
    [
        "Microsoft.Extensions.DependencyInjection.Extensions",
        "ThabeSoft.Mediator",
        "ThabeSoft.Mediator.Warppers",
        WarpperCode.Namespace
    ];

    public static string FromHandlerInfos(List<HandlerInfo> handlers)
    {
        var event_handlers_register_code = string.Join("\n\n", handlers.Select(GenerateInjectionCode));

        string code = $$"""
{{UsingCode.FromNamespaces(Usings)}}


namespace {{Namespace}}
{
    internal static class ThabeSoftMediatorDependencyInjectionExtensions
    {
        public static void AddMediatorHandlers(this IServiceCollection services)
        {
{{event_handlers_register_code}}
        }
    }
}
""";
        return code;
    }


    private static string GenerateInjectionCode(HandlerInfo info)
    {
        string warpper_full_name = WarpperCode.CreateWarpperClassName(info);

        if (info.Kind == HandlerKind.Command && info.ReturnTypeFullName is null)
        {
            return $"""
            // {info.HandlerTypeFullName}
            services.TryAddEnumerable(ServiceDescriptor.Scoped<ICommandHandler<{info.MessageTypeFullName}>, {info.HandlerTypeFullName}>());
            services.TryAddEnumerable(ServiceDescriptor.Scoped<ICommandHandlerWarpper, {warpper_full_name}>());
""";
        }
        if (info.Kind == HandlerKind.Command && info.ReturnTypeFullName is not null)
        {
            return $"""
            // {info.HandlerTypeFullName}
            services.TryAddEnumerable(ServiceDescriptor.Scoped<ICommandHandler<{info.MessageTypeFullName}, {info.ReturnTypeFullName}>, {info.HandlerTypeFullName}>());
            services.TryAddEnumerable(ServiceDescriptor.Scoped<IResponseCommandHandlerWarpper, {warpper_full_name}>());
""";
        }
        if (info.Kind == HandlerKind.Query && info.ReturnTypeFullName is not null)
        {
            return $"""
            // {info.HandlerTypeFullName}
            services.TryAddEnumerable(ServiceDescriptor.Scoped<IQueryHandler<{info.MessageTypeFullName}, {info.ReturnTypeFullName}>, {info.HandlerTypeFullName}>());
            services.TryAddEnumerable(ServiceDescriptor.Scoped<IQueryHandlerWarpper, {warpper_full_name}>());
""";
        }

        return $"""
            // {info.HandlerTypeFullName}
            services.TryAddEnumerable(ServiceDescriptor.Scoped<IEventHandler<{info.MessageTypeFullName}>, {info.HandlerTypeFullName}>());
            services.TryAddEnumerable(ServiceDescriptor.Scoped<IEventHandlerWarpper, {warpper_full_name}>());
""";
    }
}
