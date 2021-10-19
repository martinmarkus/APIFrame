using APIFrame.BackgroundService.Services.Interfaces;
using Hangfire;
using System;

namespace APIFrame.BackgroundService.Services
{
    public sealed class BackgroundJobService : IBackgroundJobService
    {
        public void FireAndForgetJob<T>(Action action)
        {
            try
            {
                BackgroundJob.Enqueue(() => action.Invoke());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void FireAndForgetJob<T>(Action<T> action, T param)
        {
            try
            {
                BackgroundJob.Enqueue(() => action.Invoke(param));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void FireAndForgetDelayedJob<T>(TimeSpan delay, Action action)
        {
            try
            {
                var jobId = BackgroundJob.Schedule(
                    () => action.Invoke(),
                    delay);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void FireAndForgetDelayedJob<T>(TimeSpan delay, Action<T> action, T param)
        {
            try
            {
                var jobId = BackgroundJob.Schedule(
                    () => action.Invoke(param),
                    delay);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void StartRecurringJob<T>(string jobId, string cronTimeValue, Action action)
        {
            try
            {
                RecurringJob.AddOrUpdate(
                    jobId,
                    () => action.Invoke(),
                    cronTimeValue);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void StartRecurringJob<T>(string jobId, string cronTimeValue, Action<T> action, T param)
        {
            try
            {
                RecurringJob.AddOrUpdate(
                    jobId,
                    () => action.Invoke(param),
                    cronTimeValue);
            } 
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
