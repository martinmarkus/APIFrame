using APIFrame.BackgroundService.Configuration;
using APIFrame.BackgroundService.Exceptions;
using APIFrame.BackgroundService.Jobs.Interfaces;
using APIFrame.BackgroundService.WireUp.Interfaces;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;

namespace APIFrame.BackgroundService.WireUp
{
    public class JobConfigurationResolver : IJobConfigurationResolver
    {
        private static ConcurrentDictionary<Type, Type> _configurations = new();

        private readonly string _configBasePath;

        public JobConfigurationResolver(IOptions<JobConfigurationOptions> options)
        {
            _configBasePath = options.Value.JobConfigPath;
        }

        public void Register<TJobType, TNewConfig>()
            where TJobType : IJob
            where TNewConfig: JobConfiguration
        {
            if (!_configurations.ContainsKey(typeof(TJobType)))
            {
                _configurations.TryAdd(typeof(TJobType), typeof(TNewConfig));
            }
        }

        public JobConfiguration Resolve<TJobType>()
            where TJobType : IJob
        {
            var exists = _configurations.TryGetValue(typeof(TJobType), out var config);
            if (!exists)
            {
                throw new MissingDependencyException(nameof(TJobType));
            }

            return default;
        }
    }
}
