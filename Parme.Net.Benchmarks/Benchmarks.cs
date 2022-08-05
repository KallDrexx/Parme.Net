using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Parme.Net.Benchmarks.Emitters;

namespace Parme.Net.Benchmarks;

[SimpleJob(RuntimeMoniker.NetCoreApp31)]
[SimpleJob(RuntimeMoniker.Net60)]
[MemoryDiagnoser]
public class Benchmarks
{
    private ParticleEmitter? _emitter;

    [GlobalSetup]
    public void CreateEmitter()
    {
        var allocator = new ParticleAllocator(1000);
        _emitter = FireEmitter.Create(new Random(), allocator);
    }


    [GlobalCleanup]
    public void DisposeEmitter()
    {
        _emitter?.Dispose();
    }

    [Benchmark]
    public void BasicTest()
    {
        _emitter!.Update(1f/60);
    }
}