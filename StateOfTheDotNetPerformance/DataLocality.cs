using BenchmarkDotNet.Attributes;
using System.Linq;

namespace StateOfTheDotNetPerformance
{
    [BenchmarkCategory("Value Types vs Reference Types")]
    [HardwareCounters(BenchmarkDotNet.Diagnosers.HardwareCounter.CacheMisses, BenchmarkDotNet.Diagnosers.HardwareCounter.LlcMisses, BenchmarkDotNet.Diagnosers.HardwareCounter.LlcReference)]
    public class DataLocality
    {
        [Params(10, 100, 1000)]
        public int Count { get; set; }

        BidRef[] arrayOfRef;
        BidVal[] arrayOfVal;

        [Setup]
        public void Setup()
        {
            arrayOfRef = Enumerable.Repeat(1, Count).Select((val, index) => new BidRef(val, index)).ToArray();
            arrayOfVal = Enumerable.Repeat(1, Count).Select((val, index) => new BidVal(val, index)).ToArray();
        }

        [Benchmark(Baseline = true)]
        public int IterateValueTypes()
        {
            int totalQuantity = 0, totalPrice = 0;

            var array = arrayOfVal;
            for (int i = 0; i < array.Length; i++)
            {
                totalQuantity += array[i].Quantity;
                totalPrice += array[i].Price;
            }

            return totalQuantity + totalPrice;
        }

        [Benchmark]
        public int IterateReferenceTypes()
        {
            int totalQuantity = 0, totalPrice = 0;

            var array = arrayOfRef;
            for (int i = 0; i < array.Length; i++)
            {
                totalQuantity += array[i].Quantity;
                totalPrice += array[i].Price;
            }

            return totalQuantity + totalPrice;
        }

        class BidRef
        {
            internal int Quantity;
            internal int Price;

            public BidRef(int quantity, int price)
            {
                Quantity = quantity;
                Price = price;
            }
        }

        struct BidVal
        {
            internal int Quantity;
            internal int Price;

            public BidVal(int quantity, int price)
            {
                Quantity = quantity;
                Price = price;
            }
        }
    }
}
