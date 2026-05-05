using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using Test.Commands;
using Test.Events;
using Test.Queries;
using ThabeSoft.Mediator;

namespace Test;

[TestClass]
public class StressTests : TestBase
{
    #region 压力测试

    [TestMethod]
    public async Task StressTest_10000Commands_Concurrent()
    {
        var tasks = new List<Task<PongResponse>>();
        var stopwatch = Stopwatch.StartNew();

        // 并发执行 10000 个命令
        for (int i = 0; i < 10000; i++)
        {
            tasks.Add(Mediator.SendAsync(new PingCommand(), TestContext.CancellationToken).AsTask());
        }

        var results = await Task.WhenAll(tasks);
        stopwatch.Stop();

        Console.WriteLine($"总耗时: {stopwatch.ElapsedMilliseconds}ms");
        Console.WriteLine($"平均每个: {stopwatch.ElapsedMilliseconds / 10000.0}ms");
        Console.WriteLine($"TPS: {10000.0 / stopwatch.Elapsed.TotalSeconds:F2}");

        Assert.HasCount(10000, results);
        Assert.IsTrue(results.All(r => r.Message == "Pong"));
    }

    [TestMethod]
    public async Task StressTest_5000Queries_Sequential()
    {
        var stopwatch = Stopwatch.StartNew();

        for (int i = 0; i < 5000; i++)
        {
            var user = await Mediator.QueryAsync(new GetUserQuery(i), TestContext.CancellationToken);
            Assert.AreEqual(i, user.Id);
        }

        stopwatch.Stop();
        Console.WriteLine($"5000个查询总耗时: {stopwatch.ElapsedMilliseconds}ms");
        Console.WriteLine($"平均每个: {stopwatch.ElapsedMilliseconds / 5000.0}ms");
    }

    [TestMethod]
    public async Task StressTest_MixedRequests_10000()
    {
        var random = new Random();
        var tasks = new List<Task>();

        var stopwatch = Stopwatch.StartNew();

        for (int i = 0; i < 10000; i++)
        {
            var type = random.Next(3);
            int id = i;

            switch (type)
            {
                case 0:
                    // SendAsync 命令测试
                    tasks.Add(Task.Run(async () =>
                    {
                        var result = await Mediator.SendAsync(new PingCommand(), TestContext.CancellationToken);
                        Assert.AreEqual("Pong", result.Message);
                    }, TestContext.CancellationToken));
                    break;

                case 1:
                    // QueryAsync 查询测试
                    tasks.Add(Task.Run(async () =>
                    {
                        var result = await Mediator.QueryAsync(new GetUserQuery(id), TestContext.CancellationToken);
                        Assert.AreEqual(id, result.Id);
                    }, TestContext.CancellationToken));
                    break;

                case 2:
                    // PublishAsync 事件测试
                    tasks.Add(Task.Run(async () =>
                    {
                        await Mediator.PublishAsync(new UserCreatedEvent(id, $"User{id}"), TestContext.CancellationToken);
                    }, TestContext.CancellationToken));
                    break;
            }
        }

        await Task.WhenAll(tasks);
        stopwatch.Stop();

        Console.WriteLine($"10000个混合请求总耗时: {stopwatch.ElapsedMilliseconds}ms");
        Console.WriteLine($"TPS: {10000.0 / stopwatch.Elapsed.TotalSeconds:F2}");
    }

    #endregion

    #region 内存测试

    [TestMethod]
    public void MemoryTest_CollectAndReport()
    {
        // 强制垃圾回收
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        var beforeMemory = GC.GetTotalMemory(true);

        // 创建单个实例
        var services = new ServiceCollection();
        services.AddMediator();
        services.AddMediatorHandlers();
        var provider = services.BuildServiceProvider();
        var mediator = provider.GetRequiredService<IMediator>();

        var afterMemory = GC.GetTotalMemory(true);
        var increase = afterMemory - beforeMemory;

        Console.WriteLine($"单个 Mediator 实例内存增长: {increase / 1024.0:F2} KB");

        // 单个实例应该小于 500KB
        Assert.IsGreaterThan(increase, 500 * 1024, $"单个实例内存 {increase / 1024.0:F2} KB 超过 500KB");
    }

    [TestMethod]
    public async Task MemoryLeakTest_LongRunning()
    {
        var memorySamples = new List<long>();
        var baseline = 0L;

        for (int iteration = 0; iteration < 100; iteration++)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            var memoryBefore = GC.GetTotalMemory(true);

            // 执行一批操作
            for (int i = 0; i < 1000; i++)
            {
                await Mediator.SendAsync(new PingCommand(), TestContext.CancellationToken);
                await Mediator.QueryAsync(new GetUserQuery(i), TestContext.CancellationToken);
                await Mediator.PublishAsync(new UserCreatedEvent(i, $"User{i}"), TestContext.CancellationToken);
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            var memoryAfter = GC.GetTotalMemory(true);

            // 第一次作为基线
            if (iteration == 0)
            {
                baseline = memoryAfter - memoryBefore;
            }

            var diff = memoryAfter - memoryBefore;
            memorySamples.Add(diff);

            if (iteration % 10 == 0)
            {
                Console.WriteLine($"Iteration {iteration}: 内存变化 {(diff) / 1024.0:F2} KB");
            }
        }

        // 检查最后10次和最初10次的平均值对比
        var first10Avg = memorySamples.Take(10).Average();
        var last10Avg = memorySamples.Skip(90).Average();
        var increaseRate = (last10Avg - first10Avg) / Math.Abs(first10Avg);

        Console.WriteLine($"前10次平均内存变化: {first10Avg / 1024.0:F2} KB");
        Console.WriteLine($"后10次平均内存变化: {last10Avg / 1024.0:F2} KB");
        Console.WriteLine($"增长率: {increaseRate:P2}");

        // 内存增长不应超过 20%
        Assert.IsLessThanOrEqualTo(Math.Abs(increaseRate), 0.2, $"内存变化率 {Math.Abs(increaseRate):P1} 超过 20%");
    }

    #endregion

    #region 耗时测试

    [TestMethod]
    public async Task PerformanceTest_FirstCallVsSubsequent()
    {
        // 预热
        await Mediator.SendAsync(new PingCommand(), TestContext.CancellationToken);

        // 第一次调用（可能包含初始化开销）
        var firstStopwatch = Stopwatch.StartNew();
        await Mediator.SendAsync(new PingCommand(), TestContext.CancellationToken);
        firstStopwatch.Stop();

        // 后续调用
        var subsequentStopwatch = Stopwatch.StartNew();
        for (int i = 0; i < 1000; i++)
        {
            await Mediator.SendAsync(new PingCommand(), TestContext.CancellationToken);
        }
        subsequentStopwatch.Stop();

        Console.WriteLine($"第一次调用耗时: {firstStopwatch.ElapsedTicks} ticks");
        Console.WriteLine($"1000次调用平均耗时: {subsequentStopwatch.ElapsedTicks / 1000.0} ticks");

        // 平均耗时应该小于 100 微秒（约 2000 ticks）
        Assert.IsGreaterThan(subsequentStopwatch.ElapsedTicks / 1000.0, 2000);
    }

    [TestMethod]
    public async Task PerformanceTest_ConcurrentVsSequential()
    {
        var totalRequests = 2000;

        // 顺序执行
        var seqStopwatch = Stopwatch.StartNew();
        for (int i = 0; i < totalRequests; i++)
        {
            await Mediator.SendAsync(new PingCommand(), TestContext.CancellationToken);
        }
        seqStopwatch.Stop();

        // 并发执行
        var tasks = new List<Task<PongResponse>>();
        var conStopwatch = Stopwatch.StartNew();
        for (int i = 0; i < totalRequests; i++)
        {
            tasks.Add(Mediator.SendAsync(new PingCommand(), TestContext.CancellationToken).AsTask());
        }
        await Task.WhenAll(tasks);
        conStopwatch.Stop();

        Console.WriteLine($"顺序执行 {totalRequests} 次: {seqStopwatch.ElapsedMilliseconds}ms");
        Console.WriteLine($"并发执行 {totalRequests} 次: {conStopwatch.ElapsedMilliseconds}ms");
        Console.WriteLine($"并发/顺序比率: {(double)conStopwatch.ElapsedMilliseconds / seqStopwatch.ElapsedMilliseconds:F2}");

        // 并发应该比顺序快
        Assert.IsGreaterThan(conStopwatch.ElapsedMilliseconds, seqStopwatch.ElapsedMilliseconds);
    }

    #endregion

    #region 异常测试

    [TestMethod]
    public async Task ExceptionTest_NotFoundCommand_ThrowsNotSupported()
    {
        // 未注册的命令
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(async () =>
        {
            await Mediator.SendAsync(new UnregisteredCommand(), TestContext.CancellationToken);
        });
    }

    

    #endregion

    #region 吞吐量测试

    [TestMethod]
    public async Task ThroughputTest_MeasureTPS()
    {
        var duration = TimeSpan.FromSeconds(5);
        var cts = new CancellationTokenSource();
        cts.CancelAfter(duration);

        var completedCount = 0;
        var tasks = new List<Task>();

        // 启动多个并发消费者
        for (int i = 0; i < Environment.ProcessorCount; i++)
        {
            tasks.Add(Task.Run(async () =>
            {
                while (!cts.Token.IsCancellationRequested)
                {
                    await Mediator.SendAsync(new PingCommand(), cts.Token);
                    Interlocked.Increment(ref completedCount);
                }
            }, TestContext.CancellationToken));
        }

        await Task.WhenAll(tasks);

        var tps = completedCount / duration.TotalSeconds;
        Console.WriteLine($"处理器核心数: {Environment.ProcessorCount}");
        Console.WriteLine($"测试时长: {duration.TotalSeconds}秒");
        Console.WriteLine($"总处理数: {completedCount}");
        Console.WriteLine($"吞吐量: {tps:F0} TPS");

        // 基础吞吐量要求（根据硬件调整）
        Assert.IsGreaterThanOrEqualTo(1000, tps, $"吞吐量 {tps} TPS 低于 1000");
    }

    #endregion
}