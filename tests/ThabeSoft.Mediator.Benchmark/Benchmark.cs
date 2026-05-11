using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Concordia;
using DispatchR.Configuration;
using DispatchR.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Perfolizer.Mathematics.Selectors;
using ThabeSoft.Mediator.Benchmark.Generated;
using ThabeSoft.Mediator.Benchmark.Handlers;
using ThabeSoft.Mediator.Benchmark.Messages;
using ThabeSoft.Mediator.Benchmark.PipelineBehaviors;
using ThabeSoft.Mediator.DependencyInjection;
using ConcordiaMediator = Concordia.IMediator;
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
    //private readonly DispatchRMediator _dispatchRMediator = default!;
    private ConcordiaMediator _concordiaMediator = default!;

    [GlobalSetup]
    public void Setup()
    {
        // ThabeSoft
        var thabesoftServices = new ServiceCollection();
        thabesoftServices.AddMediator(x => x.All().Singleton().Apply());
        _thabeSoftMediator = thabesoftServices.BuildServiceProvider().GetRequiredService<ThabeSoftMediator>();


        // Mediator
        var mediatRServices = new ServiceCollection();
        mediatRServices.AddLogging();
        mediatRServices.AddMediatR(cfg =>
        {
            cfg.Lifetime = ServiceLifetime.Singleton;
            cfg.RegisterServicesFromAssembly(typeof(Benchmark).Assembly);

            cfg.AddOpenBehavior(typeof(PipelineBehavior1<,>));
            cfg.AddOpenBehavior(typeof(PipelineBehavior2<,>));
            cfg.AddOpenBehavior(typeof(PipelineBehavior3<,>));
        });
        _mediatorMediator = mediatRServices.BuildServiceProvider().GetRequiredService<MediatorMediator>();


        // Concordia
        var concordiaServices = new ServiceCollection();
        concordiaServices.AddConcordiaHandlers();
        concordiaServices.AddConcordiaCoreServices();
        _concordiaMediator = concordiaServices.BuildServiceProvider().GetRequiredService<ConcordiaMediator>();
    }

    [Benchmark(Baseline = true)]
    public ValueTask<PongResponse> ThabeSoft()
        => _thabeSoftMediator.SendAsync(new PingRequest());

    [Benchmark]
    public Task<PongResponse> MediatR()
        => _mediatorMediator.Send(new PingRequest());

    [Benchmark]
    public Task<PongResponse> Concordia()
        => _concordiaMediator.Send(new PingRequest(), default);
}