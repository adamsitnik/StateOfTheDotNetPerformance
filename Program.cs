using System;
using BenchmarkDotNet.Running;
using System.Reflection;

namespace StateOfTheDotNetPerformance
{
    class Program
    {
        static void Main(string[] args)
        {
#if !NET46
            Console.WriteLine("If you want to see the hardware counters you need to run as .NET 4.6");
            Console.WriteLine("You can do this by running: dotnet run -f net46 -c Release");
            Console.WriteLine();
#endif

            BenchmarkSwitcher
                .FromAssembly(typeof(Program).GetTypeInfo().Assembly)
                .Run(args);
        }
    }
}