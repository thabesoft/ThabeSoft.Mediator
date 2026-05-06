using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using ThabeSoft.Mediator.SourceGenerator.Builders;
using ThabeSoft.Mediator.SourceGenerator.Models;

namespace ThabeSoft.Mediator.SourceGenerator;


[Generator]
public sealed class HandlerSourceGenerator : IIncrementalGenerator
{
    private const string RequestHandlerInterfaceName = "IRequestHandler";
    private const string NotificationHandlerInterfaceName = "INotificationHandler";
    private static readonly string[] HandlerInterfaceNames = [RequestHandlerInterfaceName, NotificationHandlerInterfaceName];


    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        //System.Diagnostics.Debugger.Launch();

        var handlers = context.SyntaxProvider
           .CreateSyntaxProvider(
               predicate: IsHandlerClass,
               transform: GetHandlerInfo)
           .Where(x => x != HandlerInfo.Empty)
           .Collect();


        context.RegisterSourceOutput(handlers, GenerateCode);
    }

    

    // 是否是处理器类
    private static bool IsHandlerClass(SyntaxNode node, CancellationToken cancellationToken)
    {
        // 不是类或者结构体
        if (node is not (ClassDeclarationSyntax or RecordDeclarationSyntax or StructDeclarationSyntax)) return false;
        // 获取类型声明
        if (node is not TypeDeclarationSyntax class_declaration) return false;

        // 排除抽象和静态
        if (class_declaration.Modifiers.Any(SyntaxKind.AbstractKeyword) || class_declaration.Modifiers.Any(SyntaxKind.StaticKeyword)) return false;

        // 没有继承列表
        if (class_declaration.BaseList is null) return false;

        return true;
    }

    // 获取处理器信息
    private static HandlerInfo GetHandlerInfo(GeneratorSyntaxContext ctx, CancellationToken cancellationToken)
    {
        // 获取语义模型（需要分析类型符号）
        var classDeclaration = (TypeDeclarationSyntax)ctx.Node;
        var declaration_symbol = ctx.SemanticModel.GetDeclaredSymbol(classDeclaration);
        if (declaration_symbol is not INamedTypeSymbol class_symbol) return HandlerInfo.Empty;

        // 查找实现的接口
        var handlerInterfaces = class_symbol.AllInterfaces
            .Where(i => HandlerInterfaceNames.Any(x => i.Name == x))
            .ToList();
        // 没有实现处理器接口
        if (handlerInterfaces.Count == 0) return HandlerInfo.Empty;

        // 是否有忽略处理器特性
        var has_att = class_symbol.GetAttributes()
            .Any(attr => attr.AttributeClass?.ToDisplayString() == "ThabeSoft.Mediator.IgnoreHandlerAttribute");
        if (has_att) return HandlerInfo.Empty;


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
        if (handler_interface_name == RequestHandlerInterfaceName)
        {
            if (handler_return_type_full_name is not null)
            {
                HandlerInfo.TryCreateRequestResponse(handler_type_full_name, message_type_full_name, handler_return_type_full_name, out var info);
                return info;
            }
            else
            {
                HandlerInfo.TryCreateRequest(handler_type_full_name, message_type_full_name, out var info);
                return info;
            }
        }
        if (handler_interface_name == NotificationHandlerInterfaceName)
        {
            HandlerInfo.TryCreateNotification(handler_type_full_name, message_type_full_name, out var info);
            return info;
        }

        return HandlerInfo.Empty;
    }


    // 生成代码
    private static void GenerateCode(SourceProductionContext context, ImmutableArray<HandlerInfo> handlerList)
    {
        // 过滤无效处理器
        var valid_handlers = handlerList.Where(x => x != HandlerInfo.Empty).ToList();
        if (valid_handlers.Count == 0) return;

        //添加代码
        TryAddSource(context, "MediatorExtensions.g.cs", new MediatorExtensionsCodeFileBuilder(valid_handlers));
        TryAddSource(context, "DependencyInjectionExtensions.g.cs", new DependencyInjectionCodeFileBuilder(valid_handlers));


        // 尝试添加
        static bool TryAddSource(SourceProductionContext context, string fileName, CodeFileBuilderBase builder)
        {
            var code = builder.Build();
            if (string.IsNullOrWhiteSpace(code)) return false;

            context.AddSource(fileName, code);
            return true;
        }
    }
}