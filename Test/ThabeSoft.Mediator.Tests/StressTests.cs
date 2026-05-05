using System.Diagnostics;
using Test.Models.Commands;
using Test.Models.Events;
using Test.Models.Queries;

namespace ThabeSoft.Mediator.Tests;


[TestClass]
public class StressTests : MediatorTestBase
{
    [DataRow(3)]
    [DataRow(5)]
    [DataRow(10)]
    [DataRow(20)]
    [DataRow(30)]
    [TestMethod]
    public async Task TPS(double durationSeconds)
    {
        var duration = TimeSpan.FromSeconds(durationSeconds);
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
}
