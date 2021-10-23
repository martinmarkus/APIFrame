using APIFrame.BackgroundService.Configuration;

namespace APIFrame.BackgroundService.WireUp.Interfaces
{
    public interface IJobOptions<TConfigType> where TConfigType : BaseJobConfiguration
    {
        TConfigType Value { get; }
    }
}
