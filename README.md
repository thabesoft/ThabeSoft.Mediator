# ThabeSoft.Mediator

[![NuGet Version](https://img.shields.io/nuget/v/ThabeSoft.Mediator)](https://www.nuget.org/packages/ThabeSoft.Mediator)
[![NuGet Downloads](https://img.shields.io/nuget/dt/ThabeSoft.Mediator)](https://www.nuget.org/packages/ThabeSoft.Mediator)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

ThabeSoft.Mediator 是一个轻量级、高性能的 .NET 中介者模式实现，支持请求/响应、通知发布订阅以及灵活的管道行为。

## 特性

- 源生成器：编译时生成注册代码，零反射开销
- ValueTask：减少异步状态机分配

## 定义消息和处理器

```C#
using ThabeSoft.Mediator;

// 1. 定义请求和响应
public class PingCommand : IRequest<PongResponse>
{
    public string Message { get; set; }
}

public class PongResponse
{
    public string Response { get; set; }
}

// 2. 实现处理器
public class PingCommandHandler : IRequestHandler<PingCommand, PongResponse>
{
    public ValueTask<PongResponse> HandleAsync(PingCommand request, CancellationToken ct)
    {
        var response = new PongResponse { Response = $"Pong: {request.Message}" };
        return new ValueTask<PongResponse>(response);
    }
}
```

## 注册服务 (IoC)

### 手动注册

```C#
DescriptorCollection Collection = new();

// 添加请求处理器
collection.AddRequestHandler<TRequestHandler, TRequest>();
// 添加请求-响应处理器
collection.AddRequestHandler<TRequestHandler, TRequest, TResponse>();
// 添加通知处理器
collection.AddNotificationHandler<TNotificationHandler, TNotification>();

// 添加请求管道行为
collection.AddRequestBehavior<TRequestPipelineBehavior, TRequest>();
// 添加请求-响应管道行为
collection.AddRequestBehavior<TRequestPipelineBehavior, TRequest, TResponse>();
// 添加通知管道行为
collection.AddNotificationHandler<TNotificationPipelineBehavior, TNotification>();

```

### 链式生命周期配置

```C#
collection.Default(ServiceLifetime.Scoped)  //设置处理器或管道行为的默认生命周期
    .Handler()             // 所有处理器
        .Scoped()          // 改为瞬态
        .Apply()           // 应用并返回父构建器
    .PipelineBehavior()    // 再次筛选所有管道行为
        .Singleton()       // 改为单例
        .Apply();          // 提交
```

## 源生成器

只需在 Program.cs 或 Startup 中调用：

```C#
services.AddMediator(x =>
{
    x.RequestHandler().Singleton();         // 将所有处理器改单例
    x.RequestHandler<TRequest>().Except();  // 查询TRequest处理器并排除
});
```

> 注意：AddMediator() 由源生成器自动生成，无需手动注册 Handler 或 Behavior

## 发送消息

```C#

public interface ISender
{
    ValueTask SendAsync<TRequest>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : IRequest;
    ValueTask<TResponse> SendAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : IRequest<TResponse>;
}

public interface IPublisher
{
    ValueTask PublishAsync<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification;
}

public interface IMediator : ISender, IPublisher;
```

## 高级配置

### 条件性排除

```C#
services.AddMediator(options =>
{
    // 排除所有处理某个特定请求的 Handler
    options.RequestHandler<PingCommand>().Except();
    
    // 排除所有 Pipeline Behavior
    options.PipelineBehavior().Except();
    
    // 排除特定通知的所有处理程序
    options.NotificationHandler<UserCreatedNotification>().Except();
});
```

### 批量生命周期修改

```C#
services.AddMediator(options =>
{
    options.RequestHandler()
           .Where(x => x.Name.EndsWith("CommandHandler"))
           .Singleton();
           
    options.NotificationPipelineBehavior()
           .Where(x => x.Name.StartsWith("Transactional"))
           .Scoped();
});
```

## 依赖

```C#
- .NET Standard 2.0 (依赖 System.Threading.Tasks.Extensions, 提供 ValueTask 支持)
- .NET Standard 2.1 无额外第三方依赖
```
