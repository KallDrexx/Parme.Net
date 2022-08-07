using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Parme.Net.Benchmarks.Emitters;

namespace Parme.Net.Benchmarks;

[SimpleJob(RuntimeMoniker.Net472)]
[SimpleJob(RuntimeMoniker.NetCoreApp31)]
[SimpleJob(RuntimeMoniker.Net60)]
[MemoryDiagnoser]
public class Benchmarks
{
    [Params(1, 100, 500, 1000, 5000)]
    public int InitialCapacity { get; set; }
    
    private ParticleEmitter? _emitter;

    [GlobalSetup]
    public void CreateEmitter()
    {
        var allocator = new ParticleAllocator(10000);
        _emitter = FireEmitter.Create(new Random(), allocator, InitialCapacity);
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