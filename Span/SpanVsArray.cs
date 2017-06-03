using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BenchmarkDotNet.Attributes;

namespace StateOfTheDotNetPerformance.Span
{
    public class SpanVsArray
    {
        [Params(
            100,
            1000)]
        public int Count { get; set; } // for smaller arrays we don't get enough of Cache Miss events

        int[] arrayField;
        Span<int> spanField; // with C# 7.2 it will be impossible to store it as a field!!!

        [GlobalSetup]
        public void Setup()
        {
            arrayField = Enumerable.Repeat(1, Count).Select((val, index) => index).ToArray();
            spanField = Enumerable.Repeat(1, Count).Select((val, index) => index).ToArray();
        }

        [Benchmark(Baseline = true)]
        public int IterateSpan()
        {
            int sum = 0;
            var span = spanField;
            for (int i = 0; i < span.Length; i++)
            {
                sum += span[i];
            }
            return sum;
        }

        [Benchmark]
        public int IterateArray()
        {
            int sum = 0;
            var array = arrayField;
            for (int i = 0; i < array.Length; i++)
            {
                sum += array[i];
            }
            return sum;
        }
    }
}
