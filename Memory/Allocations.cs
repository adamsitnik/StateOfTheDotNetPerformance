#if NET46
using System;
using System.Collections.Generic;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Exporters;
using System.Runtime;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Toolchains.CsProj;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Environments;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace StateOfTheDotNetPerformance
{
    [Config(typeof(AllocationsConfig))]
    // [RPlotExporter] // uncomment to get nice charts!
    // [CsvMeasurementsExporter] // uncomment to get nice charts!
    public class Allocations
    {
        const long MaxNoGcRegion = (64 * 1024L * 1024L); // http://mattwarren.org/2016/08/16/Preventing-dotNET-Garbage-Collections-with-the-TryStartNoGCRegion-API/

        [Params(8,
            64,
            128,
            256,
            512,
            1024,
            1024 * 4,
            1024 * 8)]
        public int SizeInBytes { get; set; }

        [IterationSetup]
        public void IterationSetup()
        {
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
        public void Allocate() => Blackhole(new byte[SizeInBytes]);

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
            Blackhole(arrayPointer);

            // I am NOT freeing the memory on Purpose
            // why? because otherwise every other benchmark run will get the same block of memory 
            // that was returned for the warmup run, and it would show that Marshall is 100x faster than new
            // Marshal.FreeHGlobal(arrayPointer);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected unsafe void Blackhole(byte* input) { }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void Blackhole<T>(T input) { }
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