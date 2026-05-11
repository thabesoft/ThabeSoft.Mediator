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
        var statements = GetPipelines(infos).Where(x => !string.IsNullOrWhiteSpace(x));
        var statements_code = string.Join(TypeBuildExtensions.NewLine + TypeBuildExtensions.NewLine, statements);

        return $$"""
    internal static partial class ThabeSoftMediatorDependencyInjectionExtensions
    {
        // 添加中介者
        public static void AddMediator(this IServiceCollection services, Action<IDescriptorCollection>? optionAction = null, ServiceLifetime mediatorLifetime = ServiceLifetime.Scoped)
        {
            // 中介者
            services.AddMediator(mediatorLifetime);
            // 处理器
            services.AddMediatorHandlers(optionAction);
            // 管道行为
            services.AddMediatorPipelineBehaviors(optionAction);

{{statements_code}}
        }
    }
""";
    }

    private static IEnumerable<string> GetPipelines(IReadOnlyCollection<TypeRegistration> infos)
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
