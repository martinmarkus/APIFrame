using APIFrame.BackgroundService.Configuration;
using APIFrame.BackgroundService.WireUp.Interfaces;

namespace APIFrame.BackgroundService.WireUp
{
    public class JobOptions<TConfigType> : IJobOptions<TConfigType> where TConfigType : BaseJobConfiguration
    {
        public TConfigType Value { get; private set; }

        public JobOptions(TConfigType config)
        {
            Value = config;
        }
    }
}
