using BenchmarkDotNet.Attributes;
using Parme.Net.Benchmarks.Emitters;

namespace Parme.Net.Benchmarks;

[MemoryDiagnoser]
public class Benchmarks
{
    private ParticleAllocator _particleAllocator;
    private ParticleEmitter _emitter;

    [GlobalSetup]
    public void CreateEmitter()
    {
        var allocator = new ParticleAllocator(1000);
        _emitter = FireEmitter.Create(new Random(), allocator);
    }


    [GlobalCleanup]
    public void DisposeEmitter()
    {
        _emitter.Dispose();
    }

    [Benchmark]
    public void BasicTest()
    {
        _emitter.Update(1f/60);
    }
}