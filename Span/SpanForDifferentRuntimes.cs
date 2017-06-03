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
        public class MultipleRuntimesConfig : ManualConfig
        {
            public MultipleRuntimesConfig()
            {
                // watch and learn how to use full power of BenchmarkDotNet!

                Add(Job.Default
                        .With(CsProjNet46Toolchain.Instance) // Span NOT supported by Runtime
                        .WithId(".NET 4.6")); 

                Add(Job.Default
                       .With(CsProjCoreToolchain.NetCoreApp11) // Span NOT supported by Runtime
                       .WithId(".NET Core 1.1"));

                /// !!! warning !!! NetCoreApp20 toolchain simply sets TargetFramework = netcoreapp2.0 in generated .csproj
                /// // so you need Visual Studio 2017 Preview 15.3 to be able to run it!
                Add(Job.Default
                       .With(CsProjCoreToolchain.NetCoreApp20) // Span SUPPORTED by Runtime
                       .WithId(".NET Core 2.0"));

                Set(new DefaultOrderProvider(SummaryOrderPolicy.SlowestToFastest));
            }
        }

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