using BenchmarkDotNet.Running;
using Microsoft.Extensions.DependencyInjection;
using ThabeSoft.Mediator;
using ThabeSoft.Mediator.Benchmark;
using ThabeSoft.Mediator.DependencyInjection;

#if RELEASE
BenchmarkRunner.Run<Benchmark>();
#else

var benchmark =  new Benchmark();
benchmark.Setup();

await benchmark.ThabeSoft();
await benchmark.MediatR();
await benchmark.Concordia();
#endif