using DispatchR.Abstractions.Send;

namespace ThabeSoft.Mediator.Benchmark.Models.DispatchR;


// DispatchR 的消息定义
// 注意：DispatchR 的 IRequest 需要指定返回类型为 ValueTask<T>[citation:1][citation:4]
public sealed class DispatchRPingCommand : IRequest<DispatchRPingCommand, ValueTask<DispatchRPongResponse>>;

public readonly record struct PDispatchRongResponse(string Message = "Pong");
public readonly record struct DispatchRPongResponse(string Message = "Pong");


public sealed class DispatchRPingCommandHandler : IRequestHandler<DispatchRPingCommand, ValueTask<DispatchRPongResponse>>
{
    public ValueTask<DispatchRPongResponse> Handle(DispatchRPingCommand request, CancellationToken cancellationToken)
    {
        return ValueTask.FromResult(new DispatchRPongResponse("Pong"));
    }
}