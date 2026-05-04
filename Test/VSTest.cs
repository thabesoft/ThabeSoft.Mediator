using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using MediatR;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using Test.Commands;
using ThabeSoft.Mediator;

using MediatorMediator = MediatR.IMediator;
using ThabeSoftMediator = ThabeSoft.Mediator.IMediator;

namespace Test;


// 1. 定义 MediatR 用的消息和处理器
public class MediatRPingCommand : IRequest<MediatRPongResponse> { }
public record MediatRPongResponse(string Message);
public class MediatRPingHandler : IRequestHandler<MediatRPingCommand, MediatRPongResponse>
{
    public Task<MediatRPongResponse> Handle(MediatRPingCommand request, CancellationToken ct)
    {
        return Task.FromResult(new MediatRPongResponse("Pong"));
    }
}

// 2. 对比测试类
[TestClass]
public class MediatorComparisonTests
{
    private IServiceProvider _myServiceProvider = default!;
    private IServiceProvider _mediatRServiceProvider = default!;


    [TestInitialize]
    public void Setup()
    {
        // 初始化 你的 Mediator
        var yourServices = new ServiceCollection();
        yourServices.AddMediator();
        yourServices.AddMediatorHandlers();
        _myServiceProvider = yourServices.BuildServiceProvider();

        // 初始化 MediatR
        var mediatRServices = new ServiceCollection();
        mediatRServices.AddLogging();
        mediatRServices.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(MediatRPingHandler).Assembly));
        _mediatRServiceProvider = mediatRServices.BuildServiceProvider();
    }

    [TestMethod]
    public async Task Throughput_YourMediator_vs_MediatR()
    {
        const int durationSeconds = 3;

        // --- 测试 ThabeSoft.Mediator ---
        var yourTps = await RunThroughputTest("ThabeSoft.Mediator", true, _myServiceProvider, durationSeconds);
        // --- 测试 MediatR ---
        var mediatRTps = await RunThroughputTest("MediatR", false, _mediatRServiceProvider, durationSeconds);

        Console.WriteLine($"\n========== 最终结果 ==========");
        Console.WriteLine($"ThabeSoft.Mediator TPS: {yourTps:F0}");
        Console.WriteLine($"MediatR TPS:            {mediatRTps:F0}");
        Console.WriteLine($"性能提升倍数:           {yourTps / mediatRTps:F2} 倍");
    }

    private async Task<double> RunThroughputTest(string name, bool isMe, IServiceProvider rootProvider, int durationSeconds)
    {
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(durationSeconds));
        long completedCount = 0;
        var tasks = new List<Task>();

        for (int i = 0; i < Environment.ProcessorCount; i++)
        {
            tasks.Add(Task.Run(async () =>
            {
                // 🟢 关键修改：每个线程创建自己的 Scope
                await using var scope = rootProvider.CreateAsyncScope();
                IServiceProvider scopedServices = scope.ServiceProvider;

                if(isMe)
                {
                    var mediator = scopedServices.GetRequiredService<ThabeSoftMediator>();

                    while (!cts.Token.IsCancellationRequested)
                    {
                        await mediator.SendAsync(new PingCommand(), cts.Token);
                        Interlocked.Increment(ref completedCount);
                    }
                }
                else
                {
                    var mediator = scopedServices.GetRequiredService<MediatorMediator>();

                    while (!cts.Token.IsCancellationRequested)
                    {
                        await mediator.Send(new MediatRPingCommand(), cts.Token);
                        Interlocked.Increment(ref completedCount);
                    }
                }

                

            }, TestContext.CancellationToken));
        }

        await Task.Delay(TimeSpan.FromSeconds(durationSeconds), TestContext.CancellationToken);
        cts.Cancel();
        await Task.WhenAll(tasks);

        var tps = completedCount / (double)durationSeconds;
        Console.WriteLine($"\n--- {name} ---");
        Console.WriteLine($"总处理数: {completedCount:N0}");
        Console.WriteLine($"耗时: {durationSeconds} 秒");
        Console.WriteLine($"TPS: {tps:F0}");

        return tps;
    }


    public TestContext TestContext { get; set; }
}