using APIFrame.BackgroundService.Services;

namespace APIFrame.BackgroundService.Jobs.Interfaces
{
    public interface IJob
    {
        BackgroundJobService BackgroundJobService { get; }

        void StartJob();
    }
}
