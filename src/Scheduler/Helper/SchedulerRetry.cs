using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutomateCore.Scheduler.Helper
{
    /// <summary>
    /// Provides retry functionality for scheduled tasks.
    /// </summary>
    internal class SchedulerRetry
    {
        private readonly int _maxRetryCount;
        private readonly TimeSpan _retryDelay;

        /// <summary>
        /// Initializes a new instance of the SchedulerRetry class.
        /// </summary>
        /// <param name="maxRetryCount">Maximum number of retry attempts.</param>
        /// <param name="retryDelay">Delay between retries.</param>
        public SchedulerRetry(int maxRetryCount = 3, TimeSpan? retryDelay = null)
        {
            _maxRetryCount = maxRetryCount;
            _retryDelay = retryDelay ?? TimeSpan.FromSeconds(10);
        }

        /// <summary>
        /// Executes a synchronous task with retry logic.
        /// </summary>
        public void RunWithRetry(Action task, DateTime startedAt, Action<Exception, DateTime> onTaskFailed = null, Action<string, DateTime> onTaskSkipped = null)
        {
            int attempt = 0;

            while (true)
            {
                try
                {
                    task();
                    return;
                }
                catch (Exception ex)
                {
                    attempt++;

                    if (attempt >= _maxRetryCount)
                    {
                        onTaskFailed?.Invoke(ex, DateTime.Now);
                        break;
                    }

                    onTaskSkipped?.Invoke($"Retry {attempt} failed. Retrying in {_retryDelay.TotalSeconds} seconds...", DateTime.Now);
                    Thread.Sleep(_retryDelay);
                }
            }
        }

        /// <summary>
        /// Executes an asynchronous task with retry logic.
        /// </summary>
        public async Task RunWithRetryAsync(Func<Task> task,
            DateTime startedAt, Action<Exception, DateTime> onTaskFailed = null, Action<string, DateTime> onTaskSkipped = null)
        {
            int attempt = 0;

            while (true)
            {
                try
                {
                    await task();
                    return;
                }
                catch (Exception ex)
                {
                    attempt++;

                    if (attempt >= _maxRetryCount)
                    {
                        onTaskFailed?.Invoke(ex, DateTime.Now);
                        break;
                    }

                    onTaskSkipped?.Invoke($"Retry {attempt} failed. Retrying in {_retryDelay.TotalSeconds} seconds...", DateTime.Now);
                    await Task.Delay(_retryDelay);
                }
            }
        }
    }
}
