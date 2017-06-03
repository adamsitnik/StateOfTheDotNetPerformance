using System;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace StateOfTheDotNetPerformance.Span
{
    public class SpanVsArray
    {
        const int Loops = 100;

        [Params(
            100,
            1000)]
        public int Count { get; set; } // for smaller arrays we don't get enough of Cache Miss events

        int[] arrayField;

        [GlobalSetup]
        public void Setup()
        {
            arrayField = Enumerable.Repeat(1, Count).Select((val, index) => index).ToArray();
        }

        [Benchmark(Baseline = true, OperationsPerInvoke = Loops)]
        public int IterateSpan()
        {
            int sum = 0;
            Span<int> span = arrayField; // implicit cast to Span, we can't have Span as a field!
            for (int i = 0; i < Loops; i++)
            {
                sum += IterateSpan(span);
            }
            return sum;
        }

        [Benchmark(OperationsPerInvoke = Loops)]
        public int IterateArray()
        {
            int sum = 0;
            var arrayVariable = arrayField;
            for (int i = 0; i < Loops; i++)
            {
                sum += IterateArray(arrayVariable);
            }
            return sum;
        }

        private int IterateSpan(Span<int> span)
        {
            int sum = 0;
            for (int i = 0; i < span.Length; i++)
            {
                sum += span[i];
            }
            return sum;
        }

        private int IterateArray(int[] array)
        {
            int sum = 0;
            for (int i = 0; i < array.Length; i++)
            {
                sum += array[i];
            }
            return sum;
        }
    }
}
