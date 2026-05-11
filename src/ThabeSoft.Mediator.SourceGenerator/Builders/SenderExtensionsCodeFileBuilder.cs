using ThabeSoft.Mediator.SourceGenerator.Extensions;
using ThabeSoft.Mediator.SourceGenerator.Models;

namespace ThabeSoft.Mediator.SourceGenerator.Builders;


/// <summary>
/// 发送者扩展类
/// </summary>
public sealed class SenderExtensionsCodeFileBuilder : ITypeSourceBuilder
{
    private readonly string _fileName = "SenderExtensions.g.cs";

    private const string _namespace = "ThabeSoft.Mediator";

    private static readonly string[] _usingNamespaces =
    [
        "System.Threading",
        "System.Threading.Tasks",
    ];


    public void Build(Microsoft.CodeAnalysis.SourceProductionContext context, IReadOnlyCollection<TypeRegistration> infos)
    {
        var content = BuildContent(infos);
        var source_text = TypeBuildExtensions.BuildDefaultTemplate(_usingNamespaces, _namespace, content);

        context.AddSource(_fileName, source_text);
    }


    private string BuildContent(IReadOnlyCollection<TypeRegistration> infos)
    {
        var statements = infos
            .Where(x => x.Kind == TypeRegistrationKind.Handler)
            .Select(GenerateInjectionCode)
            .Where(x => !string.IsNullOrEmpty(x));

        var statements_code = string.Join(TypeBuildExtensions.NewLine + TypeBuildExtensions.NewLine, statements);
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
        var input_type_name = info.InputTypeSymbol?.ToDisplayString(TypeBuildExtensions.GlobalFullName);
        var output_type_name = info.OutputTypeSymbol?.ToDisplayString(TypeBuildExtensions.GlobalFullName);


        if (info.HandlerKind == HandlerKind.RequestResponse && input_type_name is not null && output_type_name is not null)
        {
            return $$"""
        // {{info.InputTypeSymbol}}
        public static ValueTask<{{output_type_name}}> SendAsync(
            this ISender sender,
            {{input_type_name}} request,
            CancellationToken cancellationToken = default)
        {
            return sender.SendAsync<
                {{input_type_name}},
                {{output_type_name}}>
                (request, cancellationToken);
        }
""";
        }

        return string.Empty;
    }
}