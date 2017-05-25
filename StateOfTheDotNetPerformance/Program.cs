using BenchmarkDotNet.Running;
using System.Reflection;

namespace StateOfTheDotNetPerformance
{
    class Program
    {
        static void Main(string[] args)
        {
            //BenchmarkSwitcher.FromAssembly(typeof(Program).GetTypeInfo().Assembly);

            BenchmarkRunner.Run<DataLocality>();
        }
    }
}