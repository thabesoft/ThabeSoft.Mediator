namespace ThabeSoft.Mediator.SourceGenerator.Codes;


internal static class MediatorExtensionsCode
{
    public static string Namespace { get; } = "ThabeSoft.Mediator";

    public static string[] Usings { get; } =
    [
        "System.Threading",
        "System.Threading.Tasks"
    ];

    public static string FromHandlerInfos(List<HandlerInfo> handlers)
    {
        var event_handlers_register_code = string.Join("\n\n", handlers.Select(GenerateInjectionCode));

        string code = $$"""
{{GeneratorHelper.GenerateFileHead()}}

{{GeneratorHelper.GenerateUsingCode(Usings)}}


namespace {{Namespace}}
{
    public static class MediatorExtensions
    {
{{event_handlers_register_code}}
    }
}
""";
        return code;
    }


    private static string GenerateInjectionCode(HandlerInfo info)
    {
        if (info.Kind == HandlerKind.Command && info.ReturnTypeFullName is not null)
        {
            return $$"""
        // {{info.MessageTypeFullName}}
        public static ValueTask<{{info.ReturnTypeFullName}}> SendAsync(this IMediator mediator, {{info.MessageTypeFullName}} command, CancellationToken cancellationToken = default)
        {
            return mediator.SendAsync<{{info.MessageTypeFullName}}, {{info.ReturnTypeFullName}}>(command, cancellationToken);
        }
""";
        }
        if (info.Kind == HandlerKind.Query && info.ReturnTypeFullName is not null)
        {
            return $$"""
        // {{info.MessageTypeFullName}}
        public static ValueTask<{{info.ReturnTypeFullName}}> QueryAsync(this IMediator mediator, {{info.MessageTypeFullName}} query, CancellationToken cancellationToken = default)
        {
            return mediator.QueryAsync<{{info.MessageTypeFullName}}, {{info.ReturnTypeFullName}}>(query, cancellationToken);
        }
""";
        }

        return string.Empty;
    }
}
