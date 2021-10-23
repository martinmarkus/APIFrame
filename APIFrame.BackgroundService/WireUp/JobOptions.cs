using APIFrame.BackgroundService.Configuration;
using APIFrame.BackgroundService.WireUp.Interfaces;
using System;

namespace APIFrame.BackgroundService.WireUp
{
    public class JobOptions<TConfigType> : IJobOptions<TConfigType> where TConfigType : BaseJobConfiguration
    {
        public TConfigType Value { get; private set; }

        public JobOptions(IServiceProvider provider)
        {
            Value = provider.GetService(typeof(TConfigType)) as TConfigType;
        }
    }
}
