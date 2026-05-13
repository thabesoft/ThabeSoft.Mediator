using Microsoft.CodeAnalysis;
using ThabeSoft.Mediator.SourceGenerator.Extensions;
using ThabeSoft.Mediator.SourceGenerator.Models;

namespace ThabeSoft.Mediator.SourceGenerator.Builders;


/// <summary>
/// 处理器依赖注入
/// </summary>
public sealed class HandlerDependencyInjectionBuilder : ITypeSourceBuilder
{
    private readonly string _fileName = "HandlerDependencyInjection.g.cs";

    private const string _namespace = "Microsoft.Extensions.DependencyInjection";


    private static readonly string[] _usingNamespaces =
    [
        "Microsoft.Extensions.DependencyInjection.Extensions",
        "ThabeSoft.Mediator",
        "ThabeSoft.Mediator.DependencyInjection",
    ];


    public void Build(SourceProductionContext context, IReadOnlyCollection<TypeRegistration> infos)
    {
        var content = BuildSourceText(infos);
        var source_text = TypeBuildExtensions.BuildDefaultTemplate(_usingNamespaces, _namespace, content);
        context.AddSource(_fileName, source_text);
    }

    private string BuildSourceText(IReadOnlyCollection<TypeRegistration> infos)
    {
        var statements = infos
            .Where(x => x.Kind == TypeRegistrationKind.Handler)
            .Distinct()
            .Select(GenerateRegisterStatements)
            .Where(x => !string.IsNullOrEmpty(x));

        var statements_code = string.Join(TypeBuildExtensions.NewLine + TypeBuildExtensions.NewLine, statements);
        if (string.IsNullOrWhiteSpace(statements_code)) return string.Empty;

        return $$"""
    internal static partial class ThabeSoftMediatorDependencyInjectionExtensions
    {
        private static void AddMediatorHandlers(this IServiceCollection services)
        {
            services.ConfigureMediator(x =>
            {
{{statements_code}}
            });
        }
    }
""";
    }

    // 生成处理器注册代码
    private static string GenerateRegisterStatements(TypeRegistration info)
    {
        var implementation_type_name = info.ImplementationTypeSymbol.ToDisplayString(TypeBuildExtensions.GlobalFullName);
        var input_type_name = info.InputTypeSymbol?.ToDisplayString(TypeBuildExtensions.GlobalFullName);
        var output_type_name = info.OutputTypeSymbol?.ToDisplayString(TypeBuildExtensions.GlobalFullName);

        if (info.HandlerKind == HandlerKind.RequestResponse && info.InputTypeSymbol is not null && info.OutputTypeSymbol is not null)
        {
            return $"""
                // {info.ImplementationTypeSymbol}
                x.AddRequestHandler<
                    {implementation_type_name},
                    {input_type_name},
                    {output_type_name}>();
""";
        }
        if (info.HandlerKind == HandlerKind.Request && info.InputTypeSymbol is not null)
        {
            return $"""
                // {info.ImplementationTypeSymbol}
                x.AddRequestHandler<
                    {implementation_type_name},
                    {input_type_name}>();
""";
        }
        if (info.HandlerKind == HandlerKind.Notification && info.InputTypeSymbol is not null)
        {
            return $"""
                // {info.ImplementationTypeSymbol}
                x.AddNotificationHandler<
                    {implementation_type_name},
                    {input_type_name}>();
""";
        }

        return string.Empty;
    }
}
