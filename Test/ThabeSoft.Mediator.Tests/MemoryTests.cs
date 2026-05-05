using Microsoft.Extensions.DependencyInjection;
using Test.Models.Commands;

namespace ThabeSoft.Mediator.Tests;

[TestClass]
public partial class MemoryTests
{
    private const int KB = 1024;
    private const int MB = 1024 * KB;


    [DataRow(50 * KB, DisplayName = "基准: 50KB")]
    [DataRow(100 * KB, DisplayName = "基准: 100KB")]
    [DataRow(200 * KB, DisplayName = "基准: 200KB")]
    [DataRow(500 * KB, DisplayName = "基准: 500KB")]
    [TestMethod]
    public void MemoryTest_SingleMediator(int limit)
    {
        var beforeMemory = GetCleanMemory();

        var services = new ServiceCollection();
        services.AddMediator();
        services.AddMediatorHandlers();
        var provider = services.BuildServiceProvider();
        var mediator = provider.GetRequiredService<IMediator>();

        var afterMemory = GC.GetTotalMemory(true);
        var increase = afterMemory - beforeMemory;

        Console.WriteLine($"单个 Mediator 实例内存: {increase / KB:F2} KB");
        Console.WriteLine($"限制: {limit / KB:F2} KB");

        Assert.IsLessThanOrEqualTo(increase, limit, $"内存 {increase / KB:F2} KB 超过限制 {limit / KB:F2} KB");
    }


    [DataRow(10 * KB, 100, DisplayName = "10KB基准, 100次")]
    [DataRow(20 * KB, 500, DisplayName = "20KB基准, 500次")]
    [DataRow(50 * KB, 1000, DisplayName = "50KB基准, 1000次")]
    [DataRow(100 * KB, 10000, DisplayName = "100KB基准, 10000次")]
    [TestMethod]
    public void MultipleMediators(int limitPerInstance, int count)
    {
        var beforeMemory = GetCleanMemory();

        var mediators = new List<IMediator>();
        for (int i = 0; i < count; i++)
        {
            var services = new ServiceCollection();
            services.AddMediator();
            services.AddMediatorHandlers();
            var provider = services.BuildServiceProvider();

            mediators.Add(provider.GetRequiredService<IMediator>());
        }

        var afterMemory = GC.GetTotalMemory(true);
        var totalIncrease = afterMemory - beforeMemory;
        var avgIncrease = totalIncrease / count;

        Console.WriteLine($"实例数量: {count}");
        Console.WriteLine($"总内存增长: {totalIncrease / KB:F2} KB");
        Console.WriteLine($"平均每个: {avgIncrease / KB:F2} KB");
        Console.WriteLine($"限制(单): {limitPerInstance / KB:F2} KB");


        Assert.IsGreaterThan(avgIncrease, limitPerInstance, $"平均内存 {avgIncrease / KB:F2} KB 超过限制 {limitPerInstance / KB:F2} KB");
    }

    [DataRow(1, 10 * KB, DisplayName = "1次操作")]
    [DataRow(10, 50 * KB, DisplayName = "10次操作")]
    [DataRow(100, 100 * KB, DisplayName = "100次操作")]
    [DataRow(1000, 200 * KB, DisplayName = "1000次操作")]
    [TestMethod]
    public async Task MemoryTest_RepeatedSend(int iterations, int limit)
    {
        var services = new ServiceCollection();
        services.AddMediator();
        services.AddMediatorHandlers();
        var provider = services.BuildServiceProvider();
        var mediator = provider.GetRequiredService<IMediator>();

        var beforeMemory = GetCleanMemory();

        for (int i = 0; i < iterations; i++)
        {
            _ = await mediator.SendAsync(new PingCommand(), TestContext.CancellationToken);
        }

        var afterMemory = GC.GetTotalMemory(true);
        var increase = afterMemory - beforeMemory;

        Console.WriteLine($"操作次数: {iterations}");
        Console.WriteLine($"内存增长: {increase / KB:F2} KB");
        Console.WriteLine($"每次平均: {increase / iterations:F2} 字节");

        Assert.IsLessThanOrEqualTo(increase,  limit, $"内存增长 {increase / KB:F2} KB 超过限制 {limit / KB:F2} KB");
    }

    [TestMethod]
    public void MemoryTest_MinimalFootprint()
    {
        // 极限最小内存测试
        var beforeMemory = GetCleanMemory();

        // 最小配置：只注册 Mediator，不注册任何 Handler
        var services = new ServiceCollection();
        services.AddMediator();
        var provider = services.BuildServiceProvider();
        var mediator = provider.GetRequiredService<IMediator>();

        var afterMemory = GC.GetTotalMemory(true);
        var increase = afterMemory - beforeMemory;

        Console.WriteLine($"最小内存占用: {increase / KB:F2} KB");
        Console.WriteLine($"最小内存占用: {increase} 字节");

        // 期望小于 100KB
        Assert.IsLessThanOrEqualTo(increase, 100 * KB, $"最小内存占用 {increase / KB:F2} KB 超过 100KB");
    }

    [TestMethod]
    public async Task MemoryTest_LeakDetection()
    {
        // 多次创建释放，检测内存泄漏
        var initialMemory = GetCleanMemory();
        var maxMemory = initialMemory;

        for (int i = 0; i < 100; i++)
        {
            var services = new ServiceCollection();
            services.AddMediator();
            services.AddMediatorHandlers();
            var provider = services.BuildServiceProvider();
            var mediator = provider.GetRequiredService<IMediator>();

            // 执行一些操作
            for (int j = 0; j < 100; j++)
            {
                _ = await mediator.SendAsync(new PingCommand(), TestContext.CancellationToken);
            }

            var currentMemory = GC.GetTotalMemory(false);
            maxMemory = Math.Max(maxMemory, currentMemory);
        }

        // 强制 GC
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        var finalMemory = GC.GetTotalMemory(true);
        var leak = finalMemory - initialMemory;

        Console.WriteLine($"初始内存: {initialMemory / KB:F2} KB");
        Console.WriteLine($"峰值内存: {maxMemory / KB:F2} KB");
        Console.WriteLine($"最终内存: {finalMemory / KB:F2} KB");
        Console.WriteLine($"内存泄漏: {leak / KB:F2} KB");

        // 泄漏应该小于 10KB
        Assert.IsLessThan(leak, 10 * KB, $"检测到内存泄漏 {leak / KB:F2} KB");
    }

    private static long GetCleanMemory()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        return GC.GetTotalMemory(true);
    }

    public TestContext TestContext { get; set; }
}