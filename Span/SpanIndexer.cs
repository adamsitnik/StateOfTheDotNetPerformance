using BenchmarkDotNet.Attributes;
using System;
using System.Linq;

namespace StateOfTheDotNetPerformance.Span
{
    [Config(typeof(MultipleRuntimesConfig))]
    public class SpanIndexer
    {
        protected const int Loops = 100;

        protected const int Count = 1000;

        protected byte[] arrayField;

        [GlobalSetup]
        public void Setup()
        {
            arrayField = Enumerable.Repeat(1, Count).Select((val, index) => (byte)index).ToArray();
        }

        [Benchmark(OperationsPerInvoke = Loops * Count)]
        public byte SpanIndexer_Get()
        {
            Span<byte> local = arrayField; // implicit cast to Span, we can't have Span as a field!
            byte result = 0;
            for (int _ = 0; _ < Loops; _++)
            {
                for (int j = 0; j < local.Length; j++)
                {
                    result = local[j];
                }
            }
            return result;
        }

        [Benchmark(OperationsPerInvoke = Loops * Count)]
        public void SpanIndexer_Set()
        {
            Span<byte> local = arrayField; // implicit cast to Span, we can't have Span as a field!
            for (int _ = 0; _ < Loops; _++)
            {
                for (int j = 0; j < local.Length; j++)
                {
                    local[j] = byte.MaxValue;
                }
            }
        }
    }
}
