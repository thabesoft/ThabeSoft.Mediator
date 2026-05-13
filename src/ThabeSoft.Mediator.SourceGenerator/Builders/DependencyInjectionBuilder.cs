using System.Xml.Linq;
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
    ];


    public void Build(Microsoft.CodeAnalysis.SourceProductionContext context, IReadOnlyCollection<TypeRegistration> infos)
    {
        var content = BuildContent(infos);
        var source_text = TypeBuildExtensions.BuildDefaultTemplate(_usingNamespaces, _namespace, content);

        context.AddSource(_fileName, source_text);
    }

    private static string BuildContent(IReadOnlyCollection<TypeRegistration> infos)
    {

        var handler_code = AddMediatorHandlers(infos);
        var pipeline_behavior_code = AddPipelineBehaviors(infos);

        var pipelin_codes = AddPipeline(infos).Where(x => !string.IsNullOrWhiteSpace(x));
        var pipelin_code = string.Join(TypeBuildExtensions.NewLine + TypeBuildExtensions.NewLine, pipelin_codes);

        return $$"""
    internal static partial class ThabeSoftMediatorDependencyInjectionExtensions
    {
        // 添加中介者
        public static void AddMediator(this IServiceCollection services, Action<IDescriptorCollection>? optionAction = null, ServiceLifetime mediatorLifetime = ServiceLifetime.Scoped)
        {
            // 添加中介者并设置生命周期
            services.AddMediator(mediatorLifetime);

{{handler_code}}
            
{{pipeline_behavior_code}}

{{pipelin_code}}

            // 配置
            if (optionAction is not null)  services.ConfigureMediator(optionAction);
        }
    }
""";
    }

    private static string AddMediatorHandlers(IReadOnlyCollection<TypeRegistration> infos)
    {
        if (infos.Count(x => x.Kind == TypeRegistrationKind.Handler) <= 0) return string.Empty;
        return """
            // 处理器
            services.AddMediatorHandlers();
""";
    }

    private static string AddPipelineBehaviors(IReadOnlyCollection<TypeRegistration> infos)
    {
        if (infos.Count(x => x.Kind == TypeRegistrationKind.PipelineBehavior) <= 0) return string.Empty;
        return """
            // 管道行为
            services.AddMediatorPipelineBehaviors();
""";
    }

    private static IEnumerable<string> AddPipeline(IReadOnlyCollection<TypeRegistration> infos)
    {
        foreach (var handler in infos.Distinct().Where(x => x.Kind == TypeRegistrationKind.Handler).ToArray())
        {
            var behaviors = infos.Where(x => x.Kind == TypeRegistrationKind.PipelineBehavior && x.HandlerKind == handler.HandlerKind).ToArray();
            if (behaviors.Length == 0) continue;

            var className = PipelineDependencyInjectionBuilder.GetPipelineClassName(handler);
            if (string.IsNullOrWhiteSpace(className)) continue;

            yield return $"""
            // {handler.ServiceTypeSymbol}
            services.Add{className}();
""";
        }
    }
}