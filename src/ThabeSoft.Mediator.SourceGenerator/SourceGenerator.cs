using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using ThabeSoft.Mediator.SourceGenerator.Models;
using ThabeSoft.Mediator.SourceGenerator.Services.Builders;
using ThabeSoft.Mediator.SourceGenerator.Services.Parsers;

namespace ThabeSoft.Mediator.SourceGenerator;


[Generator]
public class SourceGenerator : IIncrementalGenerator
{
    private readonly ITypeParser[] _typeParsers;
    private readonly ICodeFileBuilder[] _codeFileBuilders;

    public SourceGenerator()
    {
        _typeParsers =
        [
            new RequestHandlerTypeParser(),
            new MiddlewareTypeParser()
        ];

        _codeFileBuilders =
        [
            new HandlerDependencyInjectionBuilder(),
            new SenderExtensionsCodeFileBuilder(),
            new MiddlewareDependencyInjectionBuilder()
        ];
    }


    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        //System.Diagnostics.Debugger.Launch();

        var handlers = context.SyntaxProvider
          .CreateSyntaxProvider(
              predicate: IsHandlerClass,
              transform: GetTypeInfo)
          .SelectMany((x, _) => x)
          .Collect();

        context.RegisterSourceOutput(handlers, GenerateCode);
    }


    // 筛选类
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

    // 获取类型信息
    private List<ITypeInfo> GetTypeInfo(GeneratorSyntaxContext ctx, CancellationToken cancellation)
    {
        var classDeclaration = (TypeDeclarationSyntax)ctx.Node;
        var declaration_symbol = ctx.SemanticModel.GetDeclaredSymbol(classDeclaration);
        if (declaration_symbol is not INamedTypeSymbol class_symbol) return [];

        List<ITypeInfo> infos = [];

        foreach (var interface_symbol in class_symbol.AllInterfaces)
        {
            foreach (var parser in _typeParsers)
            {
                if (!parser.TryParse(interface_symbol, class_symbol, out var info)) continue;
                if (info is null) continue;

                infos.Add(info);
            }
        }

        return infos;
    }

    // 生成代码
    private void GenerateCode(SourceProductionContext context, ImmutableArray<ITypeInfo> typeInfos)
    {
        var valid_handlers = typeInfos.Distinct().ToList();
        if (valid_handlers.Count == 0) return;

        foreach(var i in _codeFileBuilders)
        {
            var code_string = i.Build(typeInfos);
            if(string.IsNullOrWhiteSpace(code_string)) continue;

            context.AddSource(i.FileName, code_string);
        }
    }
}