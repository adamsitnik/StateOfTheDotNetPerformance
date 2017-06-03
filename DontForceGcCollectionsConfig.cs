using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;

namespace StateOfTheDotNetPerformance
{
    public class DontForceGcCollectionsConfig : ManualConfig
    {
        public DontForceGcCollectionsConfig()
        {
            Add(Job.Default
                .With(new GcMode()
                {
                    Force = false // tell BenchmarkDotNet not to force GC collections after every iteration
                }));
        }
    }
}