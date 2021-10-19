using System;

namespace APIFrame.BackgroundService.Services.Interfaces
{
    public interface IBackgroundJobService
    {
        void FireAndForgetDelayedJob<T>(TimeSpan delay, Action action);
        void FireAndForgetDelayedJob<T>(TimeSpan delay, Action<T> action, T param);
        void FireAndForgetJob<T>(Action action);
        void FireAndForgetJob<T>(Action<T> action, T param);
        void StartRecurringJob<T>(string jobId, string cronTimeValue, Action action);
        void StartRecurringJob<T>(string jobId, string cronTimeValue, Action<T> action, T param);
    }
}
