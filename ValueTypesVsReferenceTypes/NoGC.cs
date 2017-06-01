using BenchmarkDotNet.Attributes;
using System;
using BenchmarkDotNet.Attributes.Jobs;

namespace StateOfTheDotNetPerformance
{
    [BenchmarkCategory(Categories.ValueTypesVsReferenceTypes)]
    [RyuJitX64Job, LegacyJitX86Job]
    [MemoryDiagnoser]
    public class NoGC
    {
        [Benchmark(Baseline = true)]
        public ValueTuple<int, int> CreateValueTuple() => ValueTuple.Create(0, 0);

        [Benchmark]
        public Tuple<int, int> CreateTuple() => Tuple.Create(0, 0);
    }
}
