using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Microsoft.Extensions.DependencyInjection;
using Test.Console.Commands;
using MediatorMediator = MediatR.IMediator;
using ThabeSoftMediator = ThabeSoft.Mediator.IMediator;

namespace Test.Console;



[MemoryDiagnoser(true)]
[SimpleJob(RuntimeMoniker.Net10_0)]
public class MediatorBenchmark
{
    private ThabeSoftMediator _thabeSoftMediator = default!;
    private MediatorMediator _mediatorMediator = default!;

    [GlobalSetup]
    public void Setup()
    {
        var yourServices = new ServiceCollection();
        yourServices.AddMediator();
        yourServices.AddMediatorHandlers();
        _thabeSoftMediator = yourServices.BuildServiceProvider().GetRequiredService<ThabeSoftMediator>();

        var mediatRServices = new ServiceCollection();
        mediatRServices.AddLogging();
        mediatRServices.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(MediatRPingCommandHandler).Assembly));
        _mediatorMediator = mediatRServices.BuildServiceProvider().GetRequiredService<MediatorMediator>();
    }

    [Benchmark]
    public async Task<MediatRPongResponse> MediatR()
        => await _mediatorMediator.Send(new MediatRPingCommand());

    [Benchmark(Baseline = true)]
    public async Task<PongResponse> ThabeSoft()
        => await _thabeSoftMediator.SendAsync(new PingCommand());
}