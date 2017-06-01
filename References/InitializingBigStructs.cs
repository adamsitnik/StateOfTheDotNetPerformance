using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Exporters;
using BenchmarkDotNet.Attributes.Jobs;

namespace StateOfTheDotNetPerformance.References
{
    [LegacyJitX86Job, LegacyJitX64Job, RyuJitX64Job]
    //[RPlotExporter] // uncomment to get nice charts!
    //[CsvMeasurementsExporter] // uncomment to get nice charts!
    public class InitializingBigStructs
    {
        struct BigStruct
        {
            public int Int1, Int2, Int3, Int4, Int5;
        }

        private BigStruct[] array;

        [GlobalSetup]
        public void Setup()
        {
            array = new BigStruct[1000];
        }

        [Benchmark]
        public void ByValue()
        {
            for (int i = 0; i < array.Length; i++)
            {
                BigStruct value = array[i];

                value.Int1 = 1;
                value.Int2 = 2;
                value.Int3 = 3;
                value.Int4 = 4;
                value.Int5 = 5;

                array[i] = value;
            }
        }

        [Benchmark(Baseline = true)]
        public void ByReference()
        {
            for (int i = 0; i < array.Length; i++)
            {
                ref BigStruct reference = ref array[i];

                reference.Int1 = 1;
                reference.Int2 = 2;
                reference.Int3 = 3;
                reference.Int4 = 4;
                reference.Int5 = 5;
            }
        }

        [Benchmark]
        public void ByReferenceOldWay()
        {
            for (int i = 0; i < array.Length; i++)
            {
                Init(ref array[i]);
            }
        }

        // try it with: [MethodImpl(MethodImplOptions.NoInlining)]
        private void Init(ref BigStruct reference)
        {
            reference.Int1 = 1;
            reference.Int2 = 2;
            reference.Int3 = 3;
            reference.Int4 = 4;
            reference.Int5 = 5;
        }

        [Benchmark]
        public void ByReferenceUnsafeImplicit()
        {
            unsafe
            {
                fixed (BigStruct* pinned = array)
                {
                    for (int i = 0; i < array.Length; i++)
                    {
                        pinned[i].Int1 = 1;
                        pinned[i].Int2 = 2;
                        pinned[i].Int3 = 3;
                        pinned[i].Int4 = 4;
                        pinned[i].Int5 = 5;
                    }
                }
            }
        }

        [Benchmark]
        public void ByReferenceUnsafeExplicit()
        {
            unsafe
            {
                fixed (BigStruct* pinned = array)
                {
                    for (int i = 0; i < array.Length; i++)
                    {
                        (*(&pinned[i])).Int1 = 1;
                        (*(&pinned[i])).Int2 = 2;
                        (*(&pinned[i])).Int3 = 3;
                        (*(&pinned[i])).Int4 = 4;
                        (*(&pinned[i])).Int5 = 5;
                    }
                }
            }
        }

        [Benchmark]
        public void ByReferenceUnsafeExplicitExtraMethod()
        {
            unsafe
            {
                fixed (BigStruct* pinned = array)
                {
                    for (int i = 0; i < array.Length; i++)
                    {
                        Init(&pinned[i]);
                    }
                }
            }
        }

        // try it with: [MethodImpl(MethodImplOptions.NoInlining)]
        private unsafe void Init(BigStruct* pointer)
        {
            (*pointer).Int1 = 1;
            (*pointer).Int2 = 2;
            (*pointer).Int3 = 3;
            (*pointer).Int4 = 4;
            (*pointer).Int5 = 5;
        }
    }
}