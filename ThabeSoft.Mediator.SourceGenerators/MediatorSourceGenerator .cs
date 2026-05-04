using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ThabeSoft.Mediator.SourceGenerators;


[Generator]
public sealed class MediatorSourceGenerator : IIncrementalGenerator
{
    private const string CommandHandlerInterfaceName = "ICommandHandler";
    private const string QueryHandlerInterfaceName = "IQueryHandler";
    private const string EventHandlerInterfaceName = "IEventHandler";
    private static readonly string[] HandlerInterfaceNames = [CommandHandlerInterfaceName, QueryHandlerInterfaceName, EventHandlerInterfaceName];


    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        //System.Diagnostics.Debugger.Launch();

        var handlers = FindHandlers(context);
        context.RegisterSourceOutput(handlers.Collect(), GenerateCode);
    }

    // 查询所有处理器信息
    private IncrementalValuesProvider<HandlerInfo> FindHandlers(IncrementalGeneratorInitializationContext context)
    {
        return context.SyntaxProvider
           .CreateSyntaxProvider(
               predicate: IsHandlerClass,
               transform: GetHandlerInfo)
           .Where(x => x != HandlerInfo.Empty);


        // 是否是处理器类
        static bool IsHandlerClass(SyntaxNode node, CancellationToken cancellationToken)
        {
            // 只关心类声明 (class, record, struct)
            if (node is not (ClassDeclarationSyntax or RecordDeclarationSyntax or StructDeclarationSyntax))
            {
                return false;
            }

            var declaration = node as TypeDeclarationSyntax;
            // 检查是否有 BaseList (即 : SomeInterface)
            if (declaration.BaseList is null) return false;

            // 快速检查是否包含 ICommandHandler 或 IQueryHandler 或 IEventHandler
            return declaration.BaseList.Types.Any(type => HandlerInterfaceNames.Any(h => type.ToString().Contains(h)));
        }
        // 获取处理器信息
        static HandlerInfo GetHandlerInfo(GeneratorSyntaxContext ctx, CancellationToken cancellationToken)
        {
            // 获取语义模型（需要分析类型符号）
            var classDeclaration = (TypeDeclarationSyntax)ctx.Node;
            var declaration_symbol = ctx.SemanticModel.GetDeclaredSymbol(classDeclaration);
            if (declaration_symbol is not INamedTypeSymbol classSymbol)
            {
                return HandlerInfo.Empty;
            }

            // 查找实现的接口
            var handlerInterfaces = classSymbol.AllInterfaces
                .Where(i => HandlerInterfaceNames.Any(x => i.Name == x))
                .ToList();
            if (handlerInterfaces.Count == 0)
            {
                return HandlerInfo.Empty;
            }

            // 提取泛型参数
            var handler_interface = handlerInterfaces.First();
            var typeArgs = handler_interface.TypeArguments;
            if (typeArgs.Length < 1)
            {
                return HandlerInfo.Empty;
            }

            // 命令类型
            var handler_interface_name = handler_interface.Name;                                // 处理器接口名称
            var handler_type_name = classSymbol.Name;                                           // 处理器类名
            var message_type_name = typeArgs[0].ToString();                                     // 消息类型
            var handler_return_type_name = typeArgs.Length > 1 ? typeArgs[1].ToString() : null; // 返回值类型

            // 构建
            if (handler_interface_name == CommandHandlerInterfaceName)
            {
                if (handler_return_type_name is not null)
                {
                    HandlerInfo.TryCreateCommand(handler_type_name, message_type_name, handler_return_type_name, out var info);
                    return info;
                }
                else
                {
                    HandlerInfo.TryCreateCommand(handler_type_name, message_type_name, out var info);
                    return info;
                }
            }
            if (handler_interface_name == QueryHandlerInterfaceName)
            {
                HandlerInfo.TryCreateQuery(handler_type_name, message_type_name, handler_return_type_name, out var info);
                return info;
            }
            if (handler_interface_name == EventHandlerInterfaceName)
            {
                HandlerInfo.TryCreateEvent(handler_type_name, message_type_name, out var info);
                return info;
            }

            return HandlerInfo.Empty;
        }
    }
    
    
    // 生成代码
    private static void GenerateCode(SourceProductionContext context, ImmutableArray<HandlerInfo> handlerList)
    {
        // 过滤无效处理器
        var validHandlers = handlerList.Where(x => x != HandlerInfo.Empty).ToList();
        //if (validHandlers.Count == 0) return;

        // 生成 DI 扩展方法
        GenerateExtensionMethod(context, validHandlers);

        // 生成 Invoker（可选，用于事件）
        GenerateDispatcherCode(context, validHandlers);
    }

    // 生成依赖注入代码
    private static void GenerateExtensionMethod(SourceProductionContext context, List<HandlerInfo> handlers)
    {
        var event_handlers_register_code = string.Join("\n", handlers.Select(GenerateEventHandlerRegisterCode));

        string code = $$"""
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ThabeSoft.Mediator;


namespace ThabeSoft.Mediator.DependencyInjection
{
    internal static class DependencyInjectionExtensions
    {
        public static void AddMediatorHandlers(this IServiceCollection services)
        {
{{event_handlers_register_code}}
        }

        public static void AddDispatchers(this IServiceCollection services)
        {
            services.TryAddScoped<IEventDispatcher, EventDispatcher>();
            services.TryAddScoped<ICommandDispatcher, CommandDispatcher>();
            services.TryAddScoped<IQueryDispatcher, QueryDispatcher>();
        }
    }
}
""";

        context.AddSource("DependencyInjectionExtensions.g.cs", code);
    }
    // 生成事件处理器注册代码 services.AddScoped<IEventHandler<{info.MessageType}>, {info.HandlerType}>();
    private static string GenerateEventHandlerRegisterCode(HandlerInfo info)
    {
        if (info.Kind == HandlerKind.Command && info.ReturnTypeName is not null)
        {
            return $"""
            services.TryAddEnumerable(ServiceDescriptor.Scoped<ICommandHandler<{info.MessageTypeName}, {info.ReturnTypeName}>, {info.HandlerTypeName}>());
""";
        }

        if (info.Kind == HandlerKind.Command && info.ReturnTypeName is null)
        {
            return $"""
            services.TryAddEnumerable(ServiceDescriptor.Scoped<ICommandHandler<{info.MessageTypeName}>, {info.HandlerTypeName}>());
""";
        }

        if (info.Kind == HandlerKind.Query && info.ReturnTypeName is not null)
        {
            return $"""
            services.TryAddEnumerable(ServiceDescriptor.Scoped<IQueryHandler<{info.MessageTypeName}, {info.ReturnTypeName}>, {info.HandlerTypeName}>());
""";
        }

        return $"""
            services.TryAddEnumerable(ServiceDescriptor.Scoped<IEventHandler<{info.MessageTypeName}>, {info.HandlerTypeName}>());
""";
    }

    // 生成调度器代码
    private static void GenerateDispatcherCode(SourceProductionContext context, List<HandlerInfo> handlers)
    {
        var filtered = handlers.GroupBy(x => x.MessageTypeName).Select(x => x.First()).ToList();
        var command_handlers_code = string.Join("\n", filtered.Where(x => x.Kind == HandlerKind.Command && x.ReturnTypeName is null).Select(GenerateHandlerCallCode));
        var command_result_handlers_code = string.Join("\n", filtered.Where(x => x.Kind == HandlerKind.Command && x.ReturnTypeName is not null).Select(GenerateHandlerCallCode));
        var query_handlers_code = string.Join("\n", filtered.Where(x => x.Kind == HandlerKind.Query && x.ReturnTypeName is not null).Select(GenerateHandlerCallCode));
        var event_handlers_code = string.Join("\n", filtered.Where(x => x.Kind == HandlerKind.Event).Select(GenerateHandlerCallCode));

        string code = $$"""
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ThabeSoft.Mediator;


namespace ThabeSoft.Mediator
{
    internal sealed class CommandDispatcher(IServiceProvider service) : ICommandDispatcher
    {
        public async Task DispatchAsync(ICommand command, CancellationToken cancellationToken)
        {
{{command_handlers_code}}
        }

        public async Task<T> DispatchAsync<T>(ICommand<T> command, CancellationToken cancellationToken)
        {
{{command_result_handlers_code}}
            throw new NotSupportedException($"未找到命令处理器: {command.GetType().Name}");
        }
    }


    internal sealed class QueryDispatcher(IServiceProvider service) : IQueryDispatcher
    {
        public async Task<T> DispatchAsync<T>(IQuery<T> query, CancellationToken cancellationToken)
        {
{{query_handlers_code}}
            throw new NotSupportedException($"未找到查询处理器: {query.GetType().Name}");
        }
    }


    internal sealed class EventDispatcher(IServiceProvider service) : IEventDispatcher
    {
        public async Task DispatchAsync(IEvent @event, CancellationToken cancellationToken)
        {
{{event_handlers_code}}
        }
    }
}
""";
        context.AddSource("Dispatcher.g.cs", code);
    }
    // 生成事件处理器调用代码
    private static string GenerateHandlerCallCode(HandlerInfo info)
    {
        // 有返回值命令
        if (info.Kind == HandlerKind.Command && info.ReturnTypeName is not null)
        {
            return $$"""
            if (command is {{info.MessageTypeName}} c)
            {
                var handler = service.GetRequiredService<ICommandHandler<{{info.MessageTypeName}}, {{info.ReturnTypeName}}>>();
                return await handler.HandleAsync(c, cancellationToken) as T;
            }
""";
        }
        // 无返回值命令
        if (info.Kind == HandlerKind.Command && info.ReturnTypeName is null)
        {
            return $$"""
            if (command is {{info.MessageTypeName}} c)
            {
                var handler = service.GetRequiredService<ICommandHandler<{{info.MessageTypeName}}>>();
                await handler.HandleAsync(c, cancellationToken);
                return;
            }
""";
        }
        // 查询
        if (info.Kind == HandlerKind.Query && info.ReturnTypeName is not null)
        {
            return $$"""
            if (query is {{info.MessageTypeName}} q)
            {
                var handler = service.GetRequiredService<IQueryHandler<{{info.MessageTypeName}}, {{info.ReturnTypeName}}>>();
                return await handler.HandleAsync(q, cancellationToken) as T;
            }
""";
        }

        // 事件
        if (info.Kind == HandlerKind.Event && info.ReturnTypeName is null)
        {
            return $$"""
            if (@event is {{info.MessageTypeName}} e)
            {
                var handlers = service.GetServices<IEventHandler<{{info.MessageTypeName}}>>();
                foreach (var i in handlers) await i.HandleAsync(e, cancellationToken);
                return;
            }
""";
        }
        return string.Empty;
    }
}