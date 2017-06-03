using BenchmarkDotNet.Attributes;
using System;

namespace StateOfTheDotNetPerformance.Span
{
    // don't run this benchmark for .NET Core 2.0 it will fail due to safety limitations
    // (BenchmarkDotNet is using Func<T> and Span can't be generic argument)
    [MemoryDiagnoser]
    [Config(typeof(DontForceGcCollectionsConfig))]
    public class SubstringVsSubslice
    {
        public const string Text = ".NET Core: Performance Storm";

        [Benchmark]
        public string Substring() => Text.Substring(0, 9);

        [Benchmark(Baseline = true)]
        public ReadOnlySpan<char> Slice() => Text.AsSpan().Slice(0, 9);
    }
}
