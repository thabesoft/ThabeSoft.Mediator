using System.Diagnostics;

namespace ThabeSoft.Mediator.IntegrationTests.Middlewares;

public sealed class MetricsMiddleware<TReq, TResp> : IMiddleware<TReq, TResp>
{
    public async ValueTask<TResp> InvokeAsync(TReq message, NextMiddleware<TReq, TResp> next, CancellationToken cancellationToken = default)
    {
        var time = Stopwatch.GetTimestamp();
        Console.Write("计时开始{ ");
        var result = await next(message, cancellationToken);

        var elapsed = Stopwatch.GetElapsedTime(time);
        Console.Write($" :{elapsed.TotalMilliseconds}ms}}计时结束");

        return result;
    }
}