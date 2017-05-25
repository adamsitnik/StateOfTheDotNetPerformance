using BenchmarkDotNet.Attributes;
using System;

namespace StateOfTheDotNetPerformance
{
    [BenchmarkCategory("Value Types vs Reference Types")]
    [MemoryDiagnoser]
    public class NoGC
    {
        [Benchmark(Baseline = true)]
        public (int, int) CreateValueTuple() => (0, 0);

        [Benchmark]
        public Tuple<int, int> CreateTuple() => Tuple.Create(0, 0);
    }
}
