using Microsoft.CodeAnalysis;
using System.Text;
using ThabeSoft.Mediator.SourceGenerator.Extensions;
using ThabeSoft.Mediator.SourceGenerator.Models;

namespace ThabeSoft.Mediator.SourceGenerator.Builders;


internal class PipelineDependencyInjectionBuilder : ITypeSourceBuilder
{
    private static readonly string[] _usingNamespaces =
    [
        "Microsoft.Extensions.DependencyInjection",
        "Microsoft.Extensions.DependencyInjection.Extensions",
        "ThabeSoft.Mediator",
        "ThabeSoft.Mediator.DependencyInjection",
    ];

    private const string _namespace = "Microsoft.Extensions.DependencyInjection";

    // 获取管道类名称
    public static string GetPipelineClassName(TypeRegistration info)
    {
        var input = info.InputTypeSymbol?.ToDisplayString(TypeBuildExtensions.NonGenericFullName).Replace(".", "");
        var output = info.OutputTypeSymbol?.ToDisplayString(TypeBuildExtensions.NonGenericFullName).Replace(".", "");

        if (info.HandlerKind == HandlerKind.Request)
        {
            return $"{input}_RequestPipeline";
        }

        if (info.HandlerKind == HandlerKind.RequestResponse)
        {
            return $"{input}_{output}_RequestPipeline";
        }

        if (info.HandlerKind == HandlerKind.Notification)
        {
            return $"{input}_NotifcationPipeline";
        }

        return string.Empty;
    }


    public void Build(SourceProductionContext context, IReadOnlyCollection<TypeRegistration> infos)
    {
        foreach (var handler in infos.Where(x => x.Kind == TypeRegistrationKind.Handler).ToArray())
        {
            var className = GetPipelineClassName(handler);
            if (string.IsNullOrWhiteSpace(className)) continue;

            string file_name = $"Pipeline_{className}.g.cs";

            if (handler.HandlerKind == HandlerKind.RequestResponse)
            {
                var behaviors = infos.Where(x => x.Kind == TypeRegistrationKind.PipelineBehavior && x.HandlerKind == HandlerKind.RequestResponse).ToArray();
                if (behaviors.Length <= 0) continue;

                var content = GetRequestResponseContent(className, handler, behaviors);
                var source_text = TypeBuildExtensions.BuildDefaultTemplate(_usingNamespaces, _namespace, content);

                context.AddSource(file_name, source_text);
            }
        }
    }

    private static string GetRequestResponseContent(string className, TypeRegistration handler, IReadOnlyCollection<TypeRegistration> pipelineBehaviors)
    {
        var input_type_name = handler.InputTypeSymbol?.ToDisplayString(TypeBuildExtensions.GlobalFullName);
        var output_type_name = handler.OutputTypeSymbol?.ToDisplayString(TypeBuildExtensions.GlobalFullName);

        StringBuilder sb = new();

        for(int i = 1; i <= pipelineBehaviors.Count; i++)
        {
            sb.AppendLine(CreateLevel(i, pipelineBehaviors.Count));
        }

        return $$"""
    internal static partial class ThabeSoftMediatorDependencyInjectionExtensions
    {
        public static void Add{{className}}(this IServiceCollection services)
        {
            services.TryAddEnumerable(ServiceDescriptor.Transient<
                IRequestPipeline<
                    {{input_type_name}},
                    {{output_type_name}}>,
                    {{className}}Pipeline>()
            );
        }

        // 管道类
        private sealed class {{className}}Pipeline(
            // 请求处理器
            IRequestHandler<
                {{input_type_name}},
                {{output_type_name}}> handler,
            // 管道行为
            IEnumerable<IRequestPipelineBehavior<
                    {{input_type_name}}, 
                    {{output_type_name}}>> _behaviors
            ) : IRequestPipeline<
                    {{input_type_name}},
                    {{output_type_name}}>
        {
            // 请求
            private {{input_type_name}} _request;

            // 管道迭代器
            private IEnumerator<
                IRequestPipelineBehavior<
                    {{input_type_name}}, 
                    {{output_type_name}}>> _enumerator = _behaviors.GetEnumerator();

{{sb}} 

            // 清理
            private void Cleanup()
            {
                _request = null;
                _enumerator?.Dispose();
                _enumerator = null;
            }
        }
    }
""";

        string CreateLevel(int level, int maxLevel)
        {
            // 只有一个层级
            if(level == 1 && maxLevel == 1)
            {
                return $$"""
            public ValueTask<{{output_type_name}}> InvokeAsync({{input_type_name}} request, CancellationToken cancellation)
            {
                return handler.HandleAsync(request, cancellation);
            }
""";
            }

            // 多个层级的第一个
            if (level == 1 && maxLevel > 1)
            {
                return $$"""
            ValueTask<{{output_type_name}}> Level{{level}}(CancellationToken cancellation)
            {
                var result =  handler.HandleAsync(_request, cancellation);
                Cleanup();
                return result;
            }
""";
            }

            // 多个层级的最后一个
            if (level > 1 && maxLevel == level)
            {
                return $$"""
            public ValueTask<{{output_type_name}}> InvokeAsync({{input_type_name}} request, CancellationToken cancellation)
            {
                // 请求
                _request = request;
                
                if (_enumerator.MoveNext())
                {
                    return _enumerator.Current.InvokeAsync(_request, Level{{level-1}}, cancellation);
                }

                return handler.HandleAsync(_request, cancellation);
            }
""";
            }

            // 多个层级中间的
            return $$"""
            ValueTask<{{output_type_name}}> Level{{level}}(CancellationToken cancellation)
            {
                if (_enumerator.MoveNext())
                {
                    return _enumerator.Current.InvokeAsync(_request, Level{{level - 1}}, cancellation);
                }

                return handler.HandleAsync(_request, cancellation);
            }
""";
        }
    }
}
