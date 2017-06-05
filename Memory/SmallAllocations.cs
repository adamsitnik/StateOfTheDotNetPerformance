#if NET46 // .NET Core 1.1 does not support GC.TryStartNoGCRegion, .NET Core 2.0 fails with exception on my box
using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Exporters;
using System.Runtime;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Toolchains.CsProj;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Environments;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Engines;

namespace StateOfTheDotNetPerformance
{
    [Config(typeof(AllocationsConfig))]
    [RPlotExporter] // uncomment to get nice charts!
    [CsvMeasurementsExporter] // uncomment to get nice charts!
    public class SmallAllocations
    {
        const long MaxNoGcRegion = (64 * 1024L * 1024L); // http://mattwarren.org/2016/08/16/Preventing-dotNET-Garbage-Collections-with-the-TryStartNoGCRegion-API/

        [Params(8,
            16,
            32,
            64,
            128,
            256,
            512,
            1024)]
        public int SizeInBytes { get; set; }

        [IterationSetup]
        public void IterationSetup()
        {
            // the goal of this benchmark is to measure how fast allocation is, so we tell GC to rest for a while
            // to have clean results, without GC side-effects

            try
            {
                GC.TryStartNoGCRegion(MaxNoGcRegion, disallowFullBlockingGC: true);
            }
            catch // for some F... reason it fails for the first time, but works for the 2nd...
            {
                GC.TryStartNoGCRegion(MaxNoGcRegion, disallowFullBlockingGC: true);
            }
        }

        [IterationCleanup]
        public void IterationCleanup()
        {
            if (GCSettings.LatencyMode == GCLatencyMode.NoGCRegion)
                GC.EndNoGCRegion();
        }

        [Benchmark(Description = "new", Baseline = true)]
        public void Allocate() => DeadCodeEliminationHelper.KeepAliveWithoutBoxing(new byte[SizeInBytes]);

        [Benchmark(Description = "stackalloc")]
        public unsafe void AllocateWithStackalloc()
        {
            var array = stackalloc byte[SizeInBytes];
            Blackhole(array);
        }

        // [Benchmark(Description = "Marshal")] it blows up the whole system!
        public void AllocateWithMarshal()
        {
            var arrayPointer = Marshal.AllocHGlobal(SizeInBytes);
            DeadCodeEliminationHelper.KeepAliveWithoutBoxing(arrayPointer);

            // I am NOT freeing the memory on Purpose
            // why? because otherwise every other benchmark run will get the same block of memory 
            // that was returned for the warmup run, and it would show that Marshall is 100x faster than new
            // Marshal.FreeHGlobal(arrayPointer);
        }

        [MethodImpl(MethodImplOptions.NoInlining)] // no-inlining prevents from dead code elimination
        private unsafe void Blackhole(byte* input) { }
    }

    public class AllocationsConfig : ManualConfig
    {
        public AllocationsConfig()
        {
            var gcSettings = new GcMode()
            {
                Force = true, // tell BenchmarkDotNet to force GC collections after every iteration
                Server = true // we want to have the biggest Largest No GC Region possible
            };
            Jit jit = Jit.RyuJit; // we want to run for x64 only, again to have the biggest Largest No GC Region possible

            Add(Job.Default
                .With(CsProjNet46Toolchain.Instance)
                .With(gcSettings.UnfreezeCopy())
                .With(jit)
                .WithId(".NET 4.6"));

            // .NET Core 1.1 does not support GC.TryStartNoGCRegion method so we don't try it
            // .NET Core 2.0 fails when trying to call GC.TryStartNoGCRegion
        }
    }
}
#endif