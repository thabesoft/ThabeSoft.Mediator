using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Collections.Concurrent;
using System.Diagnostics;
using ThabeSoft.Mediator.DependencyInjection;
using ThabeSoft.Mediator.IntegrationTests.Messages;
using ThabeSoft.Mediator.Tests.Messages;

namespace ThabeSoft.Mediator.IntegrationTests;


/// <summary>
/// 中介者测试
/// </summary>
[TestClass]
public class MediatorTests
{
    private ServiceProvider RootProvider = default!;
    public TestContext TestContext { get; set; } = default!;



    [TestInitialize]
    public void Setup()
    {
        var services = new ServiceCollection();
        services.AddMediator();
        services.AddMediatorHandlers();
        services.AddMediatorMiddlewares();

        RootProvider = services.BuildServiceProvider();
    }

    [DataRow(123)]
    [TestMethod(DisplayName = "请求流程")]
    public async Task Request_Response(int pindId)
    {
        using var mediator = CreateMediator();

        var resutl = await mediator.SendAsync(new PingRequest(pindId), TestContext.CancellationToken);
        Assert.AreEqual(pindId, resutl.PingId);
    }



    [DataRow(1000, 10, DisplayName = "1千次10并发")]
    [DataRow(1000, 200, DisplayName = "1千次200并发")]
    [DataRow(100000, 10, DisplayName = "10万次10并发")]
    [DataRow(100000, 200, DisplayName = "10万次200并发")]
    [DataRow(10000000, 10, DisplayName = "1千万次10并发")]
    [DataRow(10000000, 200, DisplayName = "1千万次200并发")]
    [TestMethod(DisplayName = "并发发送请求")]
    public async Task Request_Response_ConcurrentThroughput(int count, int maxDegreeOfParallelism)
    {
        using var mediator = CreateMediator();
        var begin_time = Stopwatch.GetTimestamp();

        var options = new ParallelOptions
        {
            MaxDegreeOfParallelism = maxDegreeOfParallelism
        };
        var results = new ConcurrentBag<PongResponse>();

        await Parallel.ForEachAsync(Enumerable.Range(0, count), options, async (i, ct) =>
        {
            var ping_id = i;
            var result = await mediator.SendAsync(new PingRequest(ping_id), TestContext.CancellationToken);
            Assert.AreEqual(ping_id, result.PingId);
            results.Add(result);
        });

        ShowInfo(count, Stopwatch.GetElapsedTime(begin_time));
    }

    [DataRow(1000, 10, DisplayName = "1千次10并发")]
    [DataRow(1000, 200, DisplayName = "1千次200并发")]
    [DataRow(100000, 10, DisplayName = "10万次10并发")]
    [DataRow(100000, 200, DisplayName = "10万次200并发")]
    [DataRow(10000000, 10, DisplayName = "1千万次10并发")]
    [DataRow(10000000, 200, DisplayName = "1千万次200并发")]
    [TestMethod(DisplayName = "并发发送无响应请求")]
    public async Task Request_ConcurrentThroughput(int count, int maxDegreeOfParallelism)
    {
        using var mediator = CreateMediator();
        var begin_time = Stopwatch.GetTimestamp();

        var options = new ParallelOptions
        {
            MaxDegreeOfParallelism = maxDegreeOfParallelism
        };
        await Parallel.ForEachAsync(Enumerable.Range(0, count), options, async (i, ct) =>
        {
            int user_id = i;
            await mediator.SendAsync(new DeleteRequest(user_id), TestContext.CancellationToken);
        });

        ShowInfo(count, Stopwatch.GetElapsedTime(begin_time));
    }

    [DataRow(1000, 10, DisplayName = "1千次10并发")]
    [DataRow(1000, 200, DisplayName = "1千次200并发")]
    [DataRow(100000, 10, DisplayName = "10万次10并发")]
    [DataRow(100000, 200, DisplayName = "10万次200并发")]
    [DataRow(10000000, 10, DisplayName = "1千万次10并发")]
    [DataRow(10000000, 200, DisplayName = "1千万次200并发")]
    [TestMethod(DisplayName = "并发请求或者通知")]
    public async Task Mixed_Message_ConcurrentThroughput(int count, int maxDegreeOfParallelism)
    {
        using var mediator = CreateMediator();
        var begin_time = Stopwatch.GetTimestamp();
        var options = new ParallelOptions
        {
            MaxDegreeOfParallelism = maxDegreeOfParallelism
        };
        await Parallel.ForEachAsync(Enumerable.Range(0, count), options, async (i, ct) =>
        {
            int id = i;
            switch (Random.Shared.Next(3))
            {
                case 0:
                    var result = await mediator.SendAsync(new PingRequest(id), TestContext.CancellationToken);
                    Assert.AreEqual(id, result.PingId);
                    break;

                case 1:
                    await mediator.SendAsync(new DeleteRequest(id), TestContext.CancellationToken);
                    break;

                case 2:
                    await mediator.PublishAsync(new UserCreatedNotification(id, $"User{id}"), TestContext.CancellationToken);
                    break;
            }
        });

        ShowInfo(count, Stopwatch.GetElapsedTime(begin_time));
    }



    private static void ShowInfo(int count, TimeSpan elapsedTime)
    {
        Console.WriteLine($"{count}个总耗时: {elapsedTime.TotalMilliseconds}ms");
        Console.WriteLine($"平均每个: {(double)elapsedTime.TotalMilliseconds / count}ms");
        var tps = count / elapsedTime.TotalSeconds;

        Console.WriteLine($"TPS: {ToChineseNumber(tps)}");

        static string ToChineseNumber(double tps)
        {
            if (tps < 1000) return $"{tps:F0}";
            if (tps < 10000) return $"{tps:F1}";
            if (tps < 100000000) return $"{tps / 10000:F2}万";
            return $"{tps / 100000000:F2}亿";
        }
    }

    private ScopeMediator CreateMediator()
    {
        var factory = RootProvider.GetRequiredService<IServiceScopeFactory>();
        var scope = factory.CreateScope();
        return new ScopeMediator(scope);
    }
}

public sealed class ScopeMediator(IServiceScope scope) : IMediator, IDisposable
{
    private readonly IMediator _mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

    public ValueTask PublishAsync<TNotification>(TNotification notification, CancellationToken cancellationToken)
        where TNotification : INotification
    {
        return _mediator.PublishAsync(notification, cancellationToken);
    }

    public ValueTask<TResponse> SendAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken)
        where TRequest : IRequest<TResponse>
    {
        return _mediator.SendAsync<TRequest, TResponse>(request, cancellationToken);
    }
    public ValueTask SendAsync<TRequest>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : IRequest
    {
        return _mediator.SendAsync<TRequest>(request, cancellationToken);
    }


    public void Dispose()
    {
        scope.Dispose();
    }
}