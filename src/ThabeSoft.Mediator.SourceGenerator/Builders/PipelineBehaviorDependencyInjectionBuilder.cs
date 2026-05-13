using Microsoft.CodeAnalysis;
using ThabeSoft.Mediator.SourceGenerator.Extensions;
using ThabeSoft.Mediator.SourceGenerator.Models;

namespace ThabeSoft.Mediator.SourceGenerator.Builders;


internal class PipelineBehaviorDependencyInjectionBuilder : ITypeSourceBuilder
{
    private readonly string _fileName = "PipelineBehaviorDependencyInjection.g.cs";

    private const string _namespace = "Microsoft.Extensions.DependencyInjection";

    private static readonly string[] _usingNamespaces =
    [
        "Microsoft.Extensions.DependencyInjection",
        "Microsoft.Extensions.DependencyInjection.Extensions",
        "ThabeSoft.Mediator",
        "ThabeSoft.Mediator.DependencyInjection",
    ];

    public void Build(SourceProductionContext context, IReadOnlyCollection<TypeRegistration> infos)
    {
        var content = BuildContent(infos);
        var sorce_text = TypeBuildExtensions.BuildDefaultTemplate(_usingNamespaces, _namespace, content);

        context.AddSource(_fileName, sorce_text);
    }


    private string BuildContent(IReadOnlyCollection<TypeRegistration> infos)
    {
        var handlers = infos.Where(x => x.Kind == TypeRegistrationKind.Handler).ToArray();
        var behaviors = infos.Where(x => x.Kind == TypeRegistrationKind.PipelineBehavior)
            .SelectMany(behavior => handlers
                .Select(handler => behavior
                    .CreateConcretePipelineBehavior(handler.InputTypeSymbol!, handler.OutputTypeSymbol!)
                )
            );

        var statements = behaviors
            .Distinct()
            .Select(GenerateRegisterStatements)
            .Where(x => !string.IsNullOrWhiteSpace(x));

        var statements_code = string.Join(TypeBuildExtensions.NewLine, statements);
        if (string.IsNullOrWhiteSpace(statements_code)) return string.Empty;


        return $$"""
    internal static partial class ThabeSoftMediatorDependencyInjectionExtensions
    {
        private static void AddMediatorPipelineBehaviors(this IServiceCollection services)
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
        var implementation_type_name = info.ImplementationTypeSymbol.ToDisplayString(TypeBuildExtensions.GlobalNonGenericFullName);
        var input_type_name = info.InputTypeSymbol?.ToDisplayString(TypeBuildExtensions.GlobalFullName);
        var output_type_name = info.OutputTypeSymbol?.ToDisplayString(TypeBuildExtensions.GlobalFullName);


        if (info.HandlerKind == HandlerKind.RequestResponse && info.InputTypeSymbol is not null && info.OutputTypeSymbol is not null)
        {
            return $"""
                // {info.Kind}
                x.AddRequestBehavior<
                    {implementation_type_name}<
                        {input_type_name},
                        {output_type_name}>,
                    {input_type_name},
                    {output_type_name}>();
""";
        }
        if (info.HandlerKind == HandlerKind.Request && info.InputTypeSymbol is not null)
        {
            return $"""
                // {info.Kind}
                x.AddRequestBehavior<
                    {implementation_type_name}<
                        {input_type_name}>,
                    {input_type_name}>();
""";
        }
        if (info.HandlerKind == HandlerKind.Notification && info.InputTypeSymbol is not null)
        {
            return $"""
                // {info.Kind}
                x.AddNotificationBehavior<
                    {implementation_type_name}<
                        {input_type_name}>,
                    {input_type_name}>();
""";
        }

        return string.Empty;
    }
}
