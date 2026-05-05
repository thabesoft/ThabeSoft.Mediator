using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using ThabeSoft.Mediator.SourceGenerator.Codes;

namespace ThabeSoft.Mediator.SourceGenerator;


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
            if (declaration_symbol is not INamedTypeSymbol class_symbol)
            {
                return HandlerInfo.Empty;
            }

            // 查找实现的接口
            var handlerInterfaces = class_symbol.AllInterfaces
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
            var handler_interface_name = handler_interface.Name;                                     // 处理器接口名称
            var handler_type_full_name = class_symbol.ToString();                                    // 处理器类名
            var message_type_full_name = typeArgs[0].ToString();                                     // 消息类型
            var handler_return_type_full_name = typeArgs.Length > 1 ? typeArgs[1].ToString() : null; // 返回值类型

            // 构建
            if (handler_interface_name == CommandHandlerInterfaceName)
            {
                if (handler_return_type_full_name is not null)
                {
                    HandlerInfo.TryCreateCommand(handler_type_full_name, message_type_full_name, handler_return_type_full_name, out var info);
                    return info;
                }
                else
                {
                    HandlerInfo.TryCreateCommand(handler_type_full_name, message_type_full_name, out var info);
                    return info;
                }
            }
            if (handler_interface_name == QueryHandlerInterfaceName)
            {
                HandlerInfo.TryCreateQuery(handler_type_full_name, message_type_full_name, handler_return_type_full_name, out var info);
                return info;
            }
            if (handler_interface_name == EventHandlerInterfaceName)
            {
                HandlerInfo.TryCreateEvent(handler_type_full_name, message_type_full_name, out var info);
                return info;
            }

            return HandlerInfo.Empty;
        }
    }
    
    
    // 生成代码
    private static void GenerateCode(SourceProductionContext context, ImmutableArray<HandlerInfo> handlerList)
    {
        // 过滤无效处理器
        var valid_handlers = handlerList.Where(x => x != HandlerInfo.Empty).ToList();
        if (valid_handlers.Count == 0) return;

        //foreach (var i in handlerList)
        //{
        //    context.AddSource($"{i.HandlerTypeFullName}.g.cs", WarpperCode.FromHandlerInfo(i));
        //}


        context.AddSource("MediatorExtensions.g.cs", MediatorExtensionsCode.FromHandlerInfos(valid_handlers));
        context.AddSource("DependencyInjectionExtensions.g.cs", DependencyInjectionCode.FromHandlerInfos(valid_handlers));
    }
}