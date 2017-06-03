using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.CsProj;

namespace StateOfTheDotNetPerformance
{
    public class MultipleRuntimesConfig : ManualConfig
    {
        public MultipleRuntimesConfig()
        {
            // watch and learn how to use full power of BenchmarkDotNet!

            Add(Job.Default
                    .With(CsProjNet46Toolchain.Instance) // Span NOT supported by Runtime
                    .WithId(".NET 4.6"));

            Add(Job.Default
                   .With(CsProjCoreToolchain.NetCoreApp11) // Span NOT supported by Runtime
                   .WithId(".NET Core 1.1"));

            /// !!! warning !!! NetCoreApp20 toolchain simply sets TargetFramework = netcoreapp2.0 in generated .csproj
            /// // so you need Visual Studio 2017 Preview 15.3 to be able to run it!
            Add(Job.Default
                   .With(CsProjCoreToolchain.NetCoreApp20) // Span SUPPORTED by Runtime
                   .WithId(".NET Core 2.0"));
        }
    }
}