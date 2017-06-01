using System;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Jobs;

namespace StateOfTheDotNetPerformance.ValueTypesVsReferenceTypes
{
    // the host process must be .NET 4.6 to get the hardware counters, run it from console: "dotnet run -f net46 -c Release"
    [BenchmarkCategory(Categories.ValueTypesVsReferenceTypes)]
    [HardwareCounters(BenchmarkDotNet.Diagnosers.HardwareCounter.CacheMisses)]
    [CoreJob, ClrJob]
    public class DataLocality
    {
        [Params(
            1000000,
            10000000,
            100000000)]
        public int Count { get; set; } // for smaller arrays we don't get enough of Cache Miss events

        Tuple<int, int>[] arrayOfRef;
        ValueTuple<int, int>[] arrayOfVal;

        [GlobalSetup]
        public void Setup()
        {
            arrayOfRef = Enumerable.Repeat(1, Count).Select((val, index) => Tuple.Create(val, index)).ToArray();
            arrayOfVal = Enumerable.Repeat(1, Count).Select((val, index) => new ValueTuple<int, int>(val, index)).ToArray();
        }

        [Benchmark(Baseline = true)]
        public int IterateValueTypes()
        {
            int item1Sum = 0, item2Sum = 0;

            var array = arrayOfVal;
            for (int i = 0; i < array.Length; i++)
            {
                item1Sum += array[i].Item1;
                item2Sum += array[i].Item2;
            }

            return item1Sum + item2Sum;
        }

        [Benchmark]
        public int IterateReferenceTypes()
        {
            int item1Sum = 0, item2Sum = 0;

            var array = arrayOfRef;
            for (int i = 0; i < array.Length; i++)
            {
                item1Sum += array[i].Item1;
                item2Sum += array[i].Item2;
            }

            return item1Sum + item2Sum;
        }
    }
}
