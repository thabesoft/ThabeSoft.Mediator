using ThabeSoft.Mediator.SourceGenerator.Extensions;
using ThabeSoft.Mediator.SourceGenerator.Models;

namespace ThabeSoft.Mediator.SourceGenerator.Builders;


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

    protected override string BuildContentStatements(IReadOnlyCollection<TypeRegistration> infos)
    {
        var statements = infos
            .Where(x => x.Kind == TypeRegistrationKind.Handler)
            .Select(GenerateInjectionCode)
            .Where(x => !string.IsNullOrEmpty(x));

        var statements_code = string.Join(NewLine + NewLine, statements);
        if (string.IsNullOrWhiteSpace(statements_code)) return string.Empty;


        return $$"""
    public static class MediatorExtensions
    {
{{statements_code}}
    }
""";
    }

    private static string GenerateInjectionCode(TypeRegistration info)
    {
        if (info.HandlerKind == HandlerKind.RequestResponse && info.InputTypeSymbol is not null && info.OutputTypeSymbol is not null)
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