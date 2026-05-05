using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using DispatchR.Abstractions.Send;
using DispatchR.Configuration;
using DispatchR.Extensions;
using Microsoft.Extensions.DependencyInjection;
using ThabeSoft.Mediator.DependencyInjection;

using DispatchRMediator = DispatchR.IMediator;
using MediatorMediator = MediatR.IMediator;
using ThabeSoftMediator = ThabeSoft.Mediator.IMediator;
using ThabeSoft.Mediator.Benchmark.Models.DispatchR;
using ThabeSoft.Mediator.Benchmark.Models.MediatR;
using ThabeSoft.Mediator.Benchmark.Models.ThabeSoft;

namespace ThabeSoft.Mediator.Benchmark;



[MemoryDiagnoser(true)]
[SimpleJob(RuntimeMoniker.Net10_0)]
public class Benchmark
{
    private ThabeSoftMediator _thabeSoftMediator = default!;
    private MediatorMediator _mediatorMediator = default!;
    private DispatchRMediator _dispatchRMediator = default!;

    [GlobalSetup]
    public void Setup()
    {
        // ThabeSoft
        var thabesoftServices = new ServiceCollection();
        thabesoftServices.AddMediator();
        thabesoftServices.AddMediatorHandlers(x =>
        {
            x.FindAllByCommand<PingCommand, PongResponse>().Singleton();
        });
        _thabeSoftMediator = thabesoftServices.BuildServiceProvider().GetRequiredService<ThabeSoftMediator>();


        // Mediator
        var mediatRServices = new ServiceCollection();
        mediatRServices.AddLogging();
        mediatRServices.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(MediatRPingCommandHandler).Assembly));
        _mediatorMediator = mediatRServices.BuildServiceProvider().GetRequiredService<MediatorMediator>();


        // DispatchR
        var dispatchRServices = new ServiceCollection();
        dispatchRServices.AddDispatchR(new ConfigurationOptions() { IncludeHandlers = [typeof(DispatchRPingCommandHandler)] });
        //dispatchRServices.AddDispatchR(typeof(DispatchRPingCommand).Assembly, withPipelines: true, withNotifications: true);
        dispatchRServices.AddScoped<IRequestHandler<DispatchRPingCommand, ValueTask<DispatchRPongResponse>>, DispatchRPingCommandHandler>();
        _dispatchRMediator = dispatchRServices.BuildServiceProvider().GetRequiredService<DispatchRMediator>();
    }

    [Benchmark(Baseline = true)]
    public async ValueTask<PongResponse> ThabeSoft()
        => await _thabeSoftMediator.SendAsync(new PingCommand());

    [Benchmark]
    public async Task<MediatRPongResponse> MediatR()
        => await _mediatorMediator.Send(new MediatRPingCommand());

    [Benchmark]
    public async Task<DispatchRPongResponse> DispatchR()
        => await _dispatchRMediator.Send(new DispatchRPingCommand(), default);
}