using BenchmarkDotNet.Attributes;
using System;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.CsProj;
using BenchmarkDotNet.Order;

namespace StateOfTheDotNetPerformance.Span
{
    [Config(typeof(MultipleRuntimesConfig))]
    public class SpanForDifferentRuntimes
    {
        [Benchmark]
        public unsafe int StackallocAndInterate()
        {
            // this benchmark is going to be executed with Span-aware Runtime: .NET Core 2.0
            // so we can't cheat anymore and store Span in a field
            // that's why I stackallocate the memory for the benchmark (it's smallest overhead + NO GC)

            int* pointerToStack = stackalloc int[256];
            Span<int> stackMemory = new Span<int>(pointerToStack, 256);

            int sum = 0;
            for (int i = 0; i < stackMemory.Length; i++)
            {
                sum += stackMemory[i];
            }

            return sum;
        }
    }
}