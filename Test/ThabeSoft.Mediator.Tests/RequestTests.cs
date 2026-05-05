using System.Diagnostics;
using Test.Models.Commands;
using Test.Models.Events;
using Test.Models.Queries;

namespace ThabeSoft.Mediator.Tests;

/// <summary>
/// 请求测试
/// </summary>
[TestClass]
public class RequestTests : MediatorTestBase
{
    [DataRow(1000)]
    [DataRow(10000)]
    [DataRow(100000)]
    [DataRow(1000000)]
    [TestMethod]
    public async Task Commands(int count)
    {
        var tasks = new List<Task<PongResponse>>();
        var stopwatch = Stopwatch.StartNew();

        // 并发执行 10000 个命令
        for (int i = 0; i < count; i++)
        {
            tasks.Add(Mediator.SendAsync(new PingCommand(), TestContext.CancellationToken).AsTask());
        }

        var results = await Task.WhenAll(tasks);
        stopwatch.Stop();

        Console.WriteLine($"总耗时: {stopwatch.ElapsedMilliseconds}ms");
        Console.WriteLine($"平均每个: {(double)stopwatch.ElapsedMilliseconds / count}ms");
        Console.WriteLine($"TPS: {count / stopwatch.Elapsed.TotalSeconds:F2}");

        Assert.HasCount(count, results);
        Assert.IsTrue(results.All(r => r.Message == "Pong"));
    }

    [DataRow(1000)]
    [DataRow(10000)]
    [DataRow(100000)]
    [DataRow(1000000)]
    [TestMethod]
    public async Task Queries(int count)
    {
        var stopwatch = Stopwatch.StartNew();

        for (int i = 0; i < count; i++)
        {
            var user = await Mediator.QueryAsync(new GetUserQuery(i), TestContext.CancellationToken);
            Assert.AreEqual(i, user.Id);
        }

        stopwatch.Stop();
        Console.WriteLine($"5000个查询总耗时: {stopwatch.ElapsedMilliseconds}ms");
        Console.WriteLine($"平均每个: {(double)stopwatch.ElapsedMilliseconds / count}ms");
    }

    [DataRow(1000)]
    [DataRow(10000)]
    [DataRow(100000)]
    [DataRow(1000000)]
    [TestMethod]
    public async Task Mixed(int count)
    {
        var random = new Random();
        var tasks = new List<Task>();

        var stopwatch = Stopwatch.StartNew();

        for (int i = 0; i < count; i++)
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
}