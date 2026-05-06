using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using DispatchR.Configuration;
using DispatchR.Extensions;
using Microsoft.Extensions.DependencyInjection;
using ThabeSoft.Mediator.Benchmark.Handlers;
using ThabeSoft.Mediator.Benchmark.Messages;
using ThabeSoft.Mediator.DependencyInjection;

using DispatchRMediator = DispatchR.IMediator;
using MediatorMediator = MediatR.IMediator;
using ThabeSoftMediator = ThabeSoft.Mediator.IMediator;

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
            x.FindAllByRequest<PingRequest, PongResponse>().Singleton();
        });
        _thabeSoftMediator = thabesoftServices.BuildServiceProvider().GetRequiredService<ThabeSoftMediator>();


        // Mediator
        var mediatRServices = new ServiceCollection();
        mediatRServices.AddLogging();
        mediatRServices.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Benchmark).Assembly));
        _mediatorMediator = mediatRServices.BuildServiceProvider().GetRequiredService<MediatorMediator>();


        // DispatchR
        var dispatchRServices = new ServiceCollection();
        dispatchRServices.AddDispatchR(new ConfigurationOptions() { IncludeHandlers = [typeof(PingRequestHandler)] });
        dispatchRServices.AddDispatchR(typeof(Benchmark).Assembly);
        //dispatchRServices.AddScoped<IRequestHandler<PingRequest, ValueTask<PongResponse>>, PingRequestHandler>();
        _dispatchRMediator = dispatchRServices.BuildServiceProvider().GetRequiredService<DispatchRMediator>();
    }

    [Benchmark(Baseline = true)]
    public async ValueTask<PongResponse> ThabeSoft()
        => await _thabeSoftMediator.SendAsync(new PingRequest());

    [Benchmark]
    public async Task<PongResponse> MediatR()
        => await _mediatorMediator.Send(new PingRequest());

    [Benchmark]
    public async Task<PongResponse> DispatchR()
        => await _dispatchRMediator.Send(new PingRequest(), default);
}