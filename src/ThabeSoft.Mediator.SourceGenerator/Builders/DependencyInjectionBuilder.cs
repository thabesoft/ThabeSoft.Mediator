using ThabeSoft.Mediator.SourceGenerator.Extensions;
using ThabeSoft.Mediator.SourceGenerator.Models;

namespace ThabeSoft.Mediator.SourceGenerator.Builders;


/// <summary>
/// 处理器依赖注入
/// </summary>
public sealed class DependencyInjectionBuilder : ITypeSourceBuilder
{
    private readonly string _fileName = "DependencyInjection.g.cs";

    private const string _namespace = "Microsoft.Extensions.DependencyInjection";

    private static readonly string[] _usingNamespaces =
    [
        "Microsoft.Extensions.DependencyInjection.Extensions",
        "ThabeSoft.Mediator",
        "ThabeSoft.Mediator.DependencyInjection",
        "ThabeSoft.Mediator.Generated",
    ];


    public void Build(Microsoft.CodeAnalysis.SourceProductionContext context, IReadOnlyCollection<TypeRegistration> infos)
    {
        var content = BuildContent(infos);
        var source_text = TypeBuildExtensions.BuildDefaultTemplate(_usingNamespaces, _namespace, content);

        context.AddSource(_fileName, source_text);
    }

    private static string BuildContent(IReadOnlyCollection<TypeRegistration> infos)
    {
        var statements = infos.Where(x => x.Kind == TypeRegistrationKind.Handler)
            .Distinct()
            .Select(GetPipeline)
            .Where(x => !string.IsNullOrEmpty(x))
            .ToArray();

        var statements_code = string.Join(TypeBuildExtensions.NewLine + TypeBuildExtensions.NewLine, statements);
        if (string.IsNullOrWhiteSpace(statements_code)) return string.Empty;


        return $$"""
    internal static partial class ThabeSoftMediatorDependencyInjectionExtensions
    {
        // 配置中介者
        public static void ConfiguredMediator(this IServiceCollection services, Action<IDescriptorCollection>? optionAction = null)
        {
            services.AddMediatorHandlers(optionAction);
            services.AddMediatorPipelineBehaviors(optionAction);

{{statements_code}}
        }
    }
""";
    }

    private static string GetPipeline(TypeRegistration info)
    {
        if (info.Kind == TypeRegistrationKind.Handler)
        {
            var method_name = PipelineBehaviorDependencyInjectionBuilderV2.GetPipelineClassName(info);
            return $"""
            // {info.ServiceTypeSymbol}
            services.Add{method_name}();
""";
        }

        return string.Empty;
    }
}
