using ThabeSoft.Mediator.SourceGenerator.Extensions;
using ThabeSoft.Mediator.SourceGenerator.Models;

namespace ThabeSoft.Mediator.SourceGenerator.Services.Builders;


/// <summary>
/// 发送者扩展类
/// </summary>
public sealed class SenderExtensionsCodeFileBuilder : CodeFileBuilderBase
{
    public SenderExtensionsCodeFileBuilder() : base(
        fileName: "SenderExtensions.g.cs",
        @namespace: "ThabeSoft.Mediator")
    {
        AddUsingNamespace("System.Threading");
        AddUsingNamespace("System.Threading.Tasks");
    }

    protected override string BuildContentStatements(IReadOnlyCollection<ITypeInfo> typeTnfos)
    {
        var handler_infos = typeTnfos.OfType<HandlerInfo>().ToArray();
        var statements = handler_infos.Select(GenerateInjectionCode).Where(x => !string.IsNullOrEmpty(x));
        var statements_code = string.Join($"{NewLine}{NewLine}", statements);

        return $$"""
    public static class MediatorExtensions
    {
{{statements_code}}
    }
""";
    }

    private static string GenerateInjectionCode(HandlerInfo info)
    {
        if (info.Kind == HandlerKind.RequestResponse && info.OutputTypeSymbol is not null)
        {
            return $$"""
        // {{info.InputTypeSymbol}}
        public static ValueTask<{{info.OutputTypeSymbol.ToDisplayString(TypeParserExtensiosn.GlobalFullName)}}> SendAsync(
            this ISender sender,
            {{info.InputTypeSymbol}} request,
            CancellationToken cancellationToken = default)
        {
            return sender.SendAsync<
                {{info.InputTypeSymbol.ToDisplayString(TypeParserExtensiosn.GlobalFullName)}},
                {{info.OutputTypeSymbol.ToDisplayString(TypeParserExtensiosn.GlobalFullName)}}>
                (request, cancellationToken);
        }
""";
        }

        return string.Empty;
    }
}
