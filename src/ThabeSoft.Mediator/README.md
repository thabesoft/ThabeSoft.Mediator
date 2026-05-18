## 关于

基于源生成器的中介者实现

## 如何使用

### 定义消息和处理器

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

### 注册服务 (IoC)

#### 手动注册

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

#### 链式生命周期配置

```C#
collection.Default(ServiceLifetime.Scoped)  //设置处理器或管道行为的默认生命周期
    .Handler()             // 所有处理器
        .Scoped()          // 改为瞬态
        .Apply()           // 应用并返回父构建器
    .PipelineBehavior()    // 再次筛选所有管道行为
        .Singleton()       // 改为单例
        .Apply();          // 提交
```

### 源生成器

只需在 Program.cs 或 Startup 中调用：

```C#
services.AddMediator(x =>
{
    x.RequestHandler().Singleton();         // 将所有处理器改单例
    x.RequestHandler<TRequest>().Except();  // 查询TRequest处理器并排除
});
```

> 注意：AddMediator() 由源生成器自动生成，无需手动注册 Handler 或 Behavior

### 发送消息

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