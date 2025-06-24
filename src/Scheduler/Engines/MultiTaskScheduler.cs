using AutomateCore.Configuration;
using AutomateCore.Enums;
using AutomateCore.Scheduler.Interfaces;
using AutomateCore.Scheduler.Tasks;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AutomateCore.Scheduler.Engines
{
    public sealed class MultiTaskScheduler : IDisposable
    {
        #region Private Fields
        private readonly ConcurrentDictionary<string, Timer> _timers = new ConcurrentDictionary<string, Timer>();
        private readonly ConcurrentDictionary<string, ITaskExecutor> _registeredTasks = new ConcurrentDictionary<string, ITaskExecutor>();
        private readonly ConcurrentDictionary<string, SchedulerSettings> _taskSchedules = new ConcurrentDictionary<string, SchedulerSettings>();
        private readonly IScheduleCalculatorFactory _scheduleCalculatorFactory;
        private readonly SemaphoreSlim _globalLock = new SemaphoreSlim(1, 1);
        private bool _disposed;
        #endregion

        #region Events
        public event Action<string, DateTime> TaskStarted;
        public event Action<string, DateTime> TaskCompleted;
        public event Action<string, Exception, DateTime> TaskFailed;
        public event Action<string, string, DateTime> TaskSkipped;
        #endregion

        #region Constructors
        public MultiTaskScheduler(IScheduleCalculatorFactory scheduleCalculatorFactory = null)
        {
            _scheduleCalculatorFactory = scheduleCalculatorFactory ?? new ScheduleCalculatorFactory();
            LoadConfiguration();
        }

        public MultiTaskScheduler(IEnumerable<SchedulerTaskConfig> configurations,
                                IScheduleCalculatorFactory scheduleCalculatorFactory = null)
        {
            _scheduleCalculatorFactory = scheduleCalculatorFactory ?? new ScheduleCalculatorFactory();
            LoadConfiguration(configurations);
        }
        #endregion

        #region Public Methods
        public void RegisterTask(ITaskExecutor task)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));
            _registeredTasks[task.TaskName] = task;
        }

        public async Task StartAllAsync()
        {
            await _globalLock.WaitAsync().ConfigureAwait(false);
            try
            {
                foreach (var taskName in _taskSchedules.Keys)
                {
                    if (_taskSchedules[taskName].Enabled && _registeredTasks.ContainsKey(taskName))
                    {
                        await StartTaskInternalAsync(taskName).ConfigureAwait(false);
                    }
                }
            }
            finally
            {
                _globalLock.Release();
            }
        }

        public async Task StartTaskAsync(string taskName)
        {
            await _globalLock.WaitAsync().ConfigureAwait(false);
            try
            {
                await StartTaskInternalAsync(taskName).ConfigureAwait(false);
            }
            finally
            {
                _globalLock.Release();
            }
        }

        public async Task StopAllAsync()
        {
            await _globalLock.WaitAsync().ConfigureAwait(false);
            try
            {
                foreach (var timer in _timers.Values)
                {
                    timer?.Dispose();
                }
                _timers.Clear();
            }
            finally
            {
                _globalLock.Release();
            }
        }

        public async Task StopTaskAsync(string taskName)
        {
            await _globalLock.WaitAsync().ConfigureAwait(false);
            try
            {
                if (_timers.TryRemove(taskName, out var timer))
                {
                    timer?.Dispose();
                }
            }
            finally
            {
                _globalLock.Release();
            }
        }

        public async Task PauseAllAsync() => await SetAllTasksPausedAsync(true).ConfigureAwait(false);
        public async Task ResumeAllAsync() => await SetAllTasksPausedAsync(false).ConfigureAwait(false);

        public async Task PauseTaskAsync(string taskName) => await SetTaskPausedAsync(taskName, true).ConfigureAwait(false);
        public async Task ResumeTaskAsync(string taskName) => await SetTaskPausedAsync(taskName, false).ConfigureAwait(false);

        public bool IsTaskRunning(string taskName)
        {
            return _registeredTasks.TryGetValue(taskName, out var task) &&
                   task is TaskExecutor te &&
                   te.IsRunning;
        }

        public IEnumerable<string> GetRegisteredTaskNames() => _registeredTasks.Keys;
        public IEnumerable<string> GetScheduledTaskNames() => _taskSchedules.Keys;
        #endregion

        #region Private Implementation
        private void LoadConfiguration(IEnumerable<SchedulerTaskConfig> configurations = null)
        {
            if (configurations != null)
            {
                foreach (var config in configurations)
                {
                    if (config.Enabled)
                    {
                        _taskSchedules[config.TaskName] = MapToSchedulerSettings(config);
                    }
                }
            }
            else
            {
                var configSection = SchedulerConfigSection.Instance;
                if (configSection?.Tasks != null)
                {
                    foreach (TaskElement config in configSection.Tasks)
                    {
                        if (config.Enabled)
                        {
                            _taskSchedules[config.Name] = new SchedulerSettings
                            {
                                TaskName = config.Name,
                                Enabled = config.Enabled,
                                ScheduleType = config.ScheduleType,
                                Hour = config.Hour,
                                Minute = config.Minute,
                                DayOfWeek = Enum.TryParse(config.DayOfWeek, true, out DayOfWeek day) ? day : DayOfWeek.Sunday,
                                DayOfMonth = config.DayOfMonth,
                                IsPaused = config.paused,
                                DryRun = config.DryRun,
                                MaxConcurrentExecutions = config.MaxConcurrentExecutions
                            };
                        }
                    }
                }
            }
        }

        private SchedulerSettings MapToSchedulerSettings(SchedulerTaskConfig config)
        {
            return new SchedulerSettings
            {
                TaskName = config.TaskName,
                Enabled = config.Enabled,
                ScheduleType = config.ScheduleType,
                Hour = config.Hour,
                Minute = config.Minute,
                DayOfWeek = Enum.TryParse(config.DayOfWeek, true, out DayOfWeek day) ? day : DayOfWeek.Sunday,
                DayOfMonth = config.DayOfMonth,
                IsPaused = config.Paused,
                MaxConcurrentExecutions = config.MaxConcurrentExecutions
            };
        }

        private async Task StartTaskInternalAsync(string taskName)
        {
            if (!_taskSchedules.TryGetValue(taskName, out var schedule) ||
                !_registeredTasks.TryGetValue(taskName, out var task))
            {
                return;
            }

            if (!schedule.Enabled || schedule.IsPaused)
            {
                if (_timers.TryRemove(taskName, out var _timer))
                {
                    _timer?.Dispose();
                }
                return;
            }

            if (!Enum.TryParse(schedule.ScheduleType, true, out ScheduleType scheduleType))
            {
                return;
            }

            if (_timers.TryRemove(taskName, out var oldTimer))
            {
                oldTimer?.Dispose();
            }

            if (scheduleType == ScheduleType.Immediate)
            {
                await ExecuteTaskImmediatelyAsync(taskName, task).ConfigureAwait(false);
                return;
            }

            var initialDelay = CalculateInitialDelay(schedule, scheduleType);
            var interval = CalculateInterval(scheduleType);

            var timer = new Timer(async _ =>
            {
                await ExecuteTaskWithConcurrencyControlAsync(taskName, task, schedule.MaxConcurrentExecutions)
                    .ConfigureAwait(false);
            }, null, initialDelay, interval);

            _timers[taskName] = timer;
        }

        private async Task ExecuteTaskWithConcurrencyControlAsync(string taskName, ITaskExecutor task, int maxConcurrent)
        {
            if (_taskSchedules.TryGetValue(taskName, out var schedule) &&
                schedule.IsPaused)
            {
                TaskSkipped?.Invoke(taskName, "Task execution is paused.", DateTime.Now);
                return;
            }

            using (var concurrencySemaphore = maxConcurrent > 1 ? new SemaphoreSlim(maxConcurrent) : null)
            {
                bool canExecute = concurrencySemaphore == null ||
                                 await concurrencySemaphore.WaitAsync(0).ConfigureAwait(false);

                if (!canExecute)
                {
                    TaskSkipped?.Invoke(taskName, "Maximum concurrent executions reached. Skipping.", DateTime.Now);
                    return;
                }

                try
                {
                    TaskStarted?.Invoke(taskName, DateTime.Now);

                    if (task is TaskExecutor taskExecutor)
                    {
                        taskExecutor.IsRunning = true;
                    }

                    if (task is IAsyncTaskExecutor asyncTask)
                    {
                        await asyncTask.ExecuteAsync(CancellationToken.None).ConfigureAwait(false);
                    }
                    else
                    {
                        await Task.Run(() => task.Execute(CancellationToken.None)).ConfigureAwait(false);
                    }

                    TaskCompleted?.Invoke(taskName, DateTime.Now);
                }
                catch (Exception ex)
                {
                    TaskFailed?.Invoke(taskName, ex, DateTime.Now);
                }
                finally
                {
                    if (task is TaskExecutor taskExecutor)
                    {
                        taskExecutor.IsRunning = false;
                    }
                    concurrencySemaphore?.Release();
                }
            }
        }

        private async Task ExecuteTaskImmediatelyAsync(string taskName, ITaskExecutor task)
        {
            try
            {
                TaskStarted?.Invoke(taskName, DateTime.Now);

                if (task is IAsyncTaskExecutor asyncTask)
                {
                    await asyncTask.ExecuteAsync(CancellationToken.None).ConfigureAwait(false);
                }
                else
                {
                    await Task.Run(() => task.Execute(CancellationToken.None)).ConfigureAwait(false);
                }

                TaskCompleted?.Invoke(taskName, DateTime.Now);
            }
            catch (Exception ex)
            {
                TaskFailed?.Invoke(taskName, ex, DateTime.Now);
            }
        }

        private TimeSpan CalculateInitialDelay(SchedulerSettings schedule, ScheduleType scheduleType)
        {
            DateTime nextRun = _scheduleCalculatorFactory
                .GetCalculator(scheduleType)
                .GetNextOccurrence(DateTime.Now, schedule);

            TimeSpan delay = nextRun - DateTime.Now;
            return delay > TimeSpan.Zero ? delay : TimeSpan.Zero;
        }

        private static TimeSpan CalculateInterval(ScheduleType scheduleType)
        {
            switch (scheduleType)
            {
                case ScheduleType.EveryMinute:
                    return TimeSpan.FromMinutes(1);
                case ScheduleType.Every2Minutes:
                    return TimeSpan.FromMinutes(2);
                case ScheduleType.Every5Minutes:
                    return TimeSpan.FromMinutes(5);
                case ScheduleType.Every10Minutes:
                    return TimeSpan.FromMinutes(10);
                case ScheduleType.Every15Minutes:
                    return TimeSpan.FromMinutes(15);
                case ScheduleType.Every30Minutes:
                    return TimeSpan.FromMinutes(30);
                case ScheduleType.Every2Hours:
                    return TimeSpan.FromHours(2);
                case ScheduleType.Every4Hours:
                    return TimeSpan.FromHours(4);
                case ScheduleType.Every6Hours:
                    return TimeSpan.FromHours(6);
                case ScheduleType.Hourly:
                    return TimeSpan.FromHours(1);
                case ScheduleType.Daily:
                    return TimeSpan.FromDays(1);
                case ScheduleType.Weekly:
                    return TimeSpan.FromDays(7);
                case ScheduleType.Monthly:
                    return TimeSpan.FromDays(30);
                default:
                    return Timeout.InfiniteTimeSpan;
            }
        }

        private async Task SetAllTasksPausedAsync(bool paused)
        {
            await _globalLock.WaitAsync().ConfigureAwait(false);
            try
            {
                foreach (var taskName in _taskSchedules.Keys.ToList())
                {
                    if (_taskSchedules.TryGetValue(taskName, out var settings))
                    {
                        settings.IsPaused = paused;
                        _taskSchedules[taskName] = settings;
                    }
                }
            }
            finally
            {
                _globalLock.Release();
            }
        }

        private async Task SetTaskPausedAsync(string taskName, bool paused)
        {
            await _globalLock.WaitAsync().ConfigureAwait(false);
            try
            {
                if (_taskSchedules.TryGetValue(taskName, out var settings))
                {
                    settings.IsPaused = paused;
                    _taskSchedules[taskName] = settings;
                }
            }
            finally
            {
                _globalLock.Release();
            }
        }
        #endregion

        #region IDisposable Implementation
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                _globalLock.Wait();
                try
                {
                    foreach (var timer in _timers.Values)
                    {
                        timer?.Dispose();
                    }
                    _timers.Clear();
                    _globalLock.Dispose();
                }
                finally
                {
                    _globalLock.Release();
                }
            }

            _disposed = true;
        }
        #endregion
    }
}