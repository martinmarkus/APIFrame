using APIFrame.BackgroundService.Configuration;
using APIFrame.BackgroundService.Jobs.Interfaces;

namespace APIFrame.BackgroundService.WireUp.Interfaces
{
    public interface IJobConfigurationResolver
    {
        void Register<TJobType, TNewConfig>()
            where TJobType : IJob
            where TNewConfig : JobConfiguration;

        JobConfiguration Resolve<TJobType>()
            where TJobType : IJob;
    }
}
