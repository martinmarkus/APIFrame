using APIFrame.BackgroundService.Jobs.Interfaces;
using APIFrame.BackgroundService.Services;

namespace APIFrame.BackgroundService.Jobs
{
    public abstract class Job: IJob
    {
        public BackgroundJobService BackgroundJobService { get; } = new();

        public abstract void StartJob();
    }
}
