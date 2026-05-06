using ThabeSoft.Mediator.SourceGenerator.Models;

namespace ThabeSoft.Mediator.SourceGenerator.Builders;


/// <summary>
/// 中介者扩展类代码文件构建
/// </summary>
internal class MediatorExtensionsCodeFileBuilder : CodeFileBuilderBase
{
    private readonly IReadOnlyCollection<HandlerInfo> _handlerInfos;

    public MediatorExtensionsCodeFileBuilder(IReadOnlyCollection<HandlerInfo> handlerInfos): base("ThabeSoft.Mediator")
    {
        _handlerInfos = handlerInfos;
        AddUsingNamespace("System.Threading");
        AddUsingNamespace("System.Threading.Tasks");
    }
    protected override string BuildContentStatements()
    {
        var selected = _handlerInfos.Where(x => x.Kind == HandlerKind.RequestResponse).ToArray();
        if (selected.Length <= 0) return string.Empty;

        var event_handlers_register_code = string.Join($"{NewLine}{NewLine}", selected.Select(GenerateInjectionCode));

        return $$"""
    public static class MediatorExtensions
    {
{{event_handlers_register_code}}
    }
""";
    }

    private static string GenerateInjectionCode(HandlerInfo info)
    {
        if (info.Kind != HandlerKind.RequestResponse) return string.Empty;

        return $$"""
        // {{info.InputTypeFullName}}
        public static ValueTask<{{info.OutputTypeFullName}}> SendAsync(this IMediator mediator, {{info.InputTypeFullName}} request, CancellationToken cancellationToken = default)
        {
            return mediator.SendAsync<{{info.InputTypeFullName}}, {{info.OutputTypeFullName}}>(request, cancellationToken);
        }
""";
    }
}
