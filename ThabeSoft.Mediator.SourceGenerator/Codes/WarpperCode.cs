namespace ThabeSoft.Mediator.SourceGenerator.Codes;

internal static class WarpperCode
{
    public static string Namespace { get; } = "ThabeSoft.Mediator.SourceGenerator.Warppers";
    public static string[] Usings { get; } =
    [
        "System",
        "System.Threading",
        "System.Threading.Tasks",
        "Microsoft.Extensions.DependencyInjection",
        "ThabeSoft.Mediator",
        "ThabeSoft.Mediator.Warppers",
    ];

    public static string FromHandlerInfo(HandlerInfo info)
    {
        string warpper_full_name = CreateWarpperClassName(info);

        if (info.Kind == HandlerKind.Command && info.ReturnTypeFullName is not null)
        {
            return GenerateResponseCommandCode(warpper_full_name, info.MessageTypeFullName, info.ReturnTypeFullName);
        }

        if (info.Kind == HandlerKind.Command && info.ReturnTypeFullName is null)
        {
            return GenerateCommandCode(warpper_full_name, info.MessageTypeFullName);
        }

        if (info.Kind == HandlerKind.Query && info.ReturnTypeFullName is not null)
        {
            return GenerateQueryCode(warpper_full_name, info.MessageTypeFullName, info.ReturnTypeFullName);
        }

        if (info.Kind == HandlerKind.Event && info.ReturnTypeFullName is null)
        {
            return GenerateEventCode(warpper_full_name, info.MessageTypeFullName);
        }

        return string.Empty;
    }

    public static string CreateWarpperClassName(HandlerInfo info)
    {
        return $"{info.HandlerTypeFullName.Replace(".", string.Empty).Trim()}Warpper";
    }


    // 生成命令执行器包装代码
    private static string GenerateCommandCode(string warpperName, string messageTypeFullName)
    {
        return $$"""
{{UsingCode.FromNamespaces(Usings)}}


namespace {{Namespace}}
{
    internal sealed class {{warpperName}}(IServiceProvider services) : ICommandHandlerWarpper
    {
        private ICommandHandler<{{messageTypeFullName}}> handler;

        public Type MessageType { get; } = typeof({{messageTypeFullName}});

        public async Task HandleAsync(ICommand command, CancellationToken cancellationToken = default)
        {
            if (command is not {{messageTypeFullName}} c) throw new NotSupportedException();

            if (handler is null)
            {
                handler = services.GetRequiredService<ICommandHandler<{{messageTypeFullName}}>>();
            }

            await handler.HandleAsync(c, cancellationToken);
        }
    }
}
""";
    }
    // 生成响应命令执行器包装代码
    private static string GenerateResponseCommandCode(string warpperName, string messageTypeFullName, string responseTypeName)
    {
        return $$"""
{{UsingCode.FromNamespaces(Usings)}}


namespace {{Namespace}}
{
    internal sealed class {{warpperName}}(IServiceProvider services) : IResponseCommandHandlerWarpper
    {
        private ICommandHandler<{{messageTypeFullName}}, {{responseTypeName}}> handler;

        public Type MessageType { get; } = typeof({{messageTypeFullName}});

        public async Task<TResult> HandleAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default)
        {
            if (command is not {{messageTypeFullName}} c) throw new NotSupportedException();

            if(handler is null) 
            {
                if (typeof({{responseTypeName}}) != typeof(TResult)) throw new NotSupportedException();
                handler = services.GetRequiredService<ICommandHandler<{{messageTypeFullName}}, {{responseTypeName}}>>();
            }
         
            if (await handler.HandleAsync(c, cancellationToken) is TResult result) return result;
            throw new InvalidOperationException("类型匹配已通过，不应执行此处");
        }
    }
}
""";
    }
    // 生成查询执行器包装代码
    private static string GenerateQueryCode(string warpperName, string messageTypeFullName, string responseTypeName)
    {
        return $$"""
{{UsingCode.FromNamespaces(Usings)}}


namespace {{Namespace}}
{
    internal sealed class {{warpperName}}(IServiceProvider services) : IQueryHandlerWarpper
    {
        private IQueryHandler<{{messageTypeFullName}}, {{responseTypeName}}> handler;

        public Type MessageType { get; } = typeof({{messageTypeFullName}});

        public async Task<TResult> HandleAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default)
        {
            if (query is not {{messageTypeFullName}} q) throw new NotSupportedException();

            if (handler is null)
            {
                if (typeof({{responseTypeName}}) != typeof(TResult)) throw new NotSupportedException();
                handler = services.GetRequiredService<IQueryHandler<{{messageTypeFullName}}, {{responseTypeName}}>>();
            }
            
            if (await handler.HandleAsync(q, cancellationToken) is TResult result) return result;
            throw new InvalidOperationException("类型匹配已通过，不应执行此处");
        }
    }
}
""";
    }
    // 生成事件执行器包装代码
    private static string GenerateEventCode(string warpperName, string messageTypeFullName)
    {
        return $$"""
{{UsingCode.FromNamespaces(Usings)}}


namespace {{Namespace}}
{
    internal sealed class {{warpperName}}(IServiceProvider services) : IEventHandlerWarpper
    {
        private IEventHandler<{{messageTypeFullName}}> handler;

        public Type MessageType { get; } = typeof({{messageTypeFullName}});

        public async Task HandleAsync(IEvent @event, CancellationToken cancellationToken = default)
        {
            if (@event is not {{messageTypeFullName}} e) throw new NotSupportedException();

            if(handler is null)
            {
                handler = services.GetRequiredService<IEventHandler<{{messageTypeFullName}}>>();
            }

            await handler.HandleAsync(e, cancellationToken);
        }
    }
}
""";
    }
}
