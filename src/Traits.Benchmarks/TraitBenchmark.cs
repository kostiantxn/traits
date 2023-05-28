using BenchmarkDotNet.Attributes;
using Traits.Benchmarks.Subjects;

namespace Traits.Benchmarks;

public class TraitBenchmark
{
    private IHash<object> _hash;
    private object _value;

    [GlobalSetup]
    public void Setup()
    {
        _value = new();
        _hash = new ObjectHash();
    }

    [Benchmark]
    public void Raw()
    {
        var _ = _value.GetHashCode();
    }

    [Benchmark(Baseline = true)]
    public void Instance()
    {
        var _ = _hash.Of(_value);
    }

    [Benchmark]
    public void Facade()
    {
        var _ = Hash.Of(_value);
    }
}