using BenchmarkDotNet.Attributes;

namespace StateOfTheDotNetPerformance.Span
{
    public class SpanVsArray_Indexer : SpanIndexer
    {
        [Benchmark(OperationsPerInvoke = Loops * Count)]
        public byte ArrayIndexer_Get()
        {
            var local = arrayField;
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
        public void ArrayIndexer_Set()
        {
            var local = arrayField;
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
