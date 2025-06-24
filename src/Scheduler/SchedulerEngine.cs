using AutomateCore.Configuration;
using AutomateCore.Enums;
using AutomateCore.Scheduler.Helper;
using AutomateCore.Scheduler.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AutomateCore.Scheduler
{
    [DebuggerStepThrough]
    public class SchedulerEngine : ISchedulerEngine, IDisposable
    {
        #region "Private Properties"
        private readonly ConcurrentBag<System.Threading.Timer> _timers = new ConcurrentBag<System.Threading.Timer>();
        private readonly IReadOnlyCollection<SchedulerSettings> _schedules;
        private readonly Action _taskToRun;
        private readonly Func<Task> _asyncTaskToRun;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private readonly IScheduleCalculatorFactory _scheduleCalculatorFactory;
        private volatile int _isRunning; // Using int for atomic operations
        private volatile int _isPaused; // Using int for atomic operations
        private readonly Action<DateTime> _onTaskStarted;
        private readonly Action<DateTime> _onTaskCompleted;
        private readonly Action<Exception, DateTime> _onTaskFailed;
        private readonly Action<string, DateTime> _onTaskSkipped;
        private readonly SchedulerRetry _retryHelper = new SchedulerRetry();
        private bool _disposed;
        private bool _isDryRun;
        #endregion

        #region "Public Properties"
        public bool IsRunning => _isRunning == 1;
        public bool IsPaused => _isPaused == 1;
        #endregion

        #region "CTOR"
        public SchedulerEngine(Action taskToRun, Action<DateTime> onTaskStarted = null,
            Action<DateTime> onTaskCompleted = null, Action<Exception, DateTime> onTaskFailed = null,
            Action<string, DateTime> onTaskSkipped = null)
            : this(onTaskStarted, onTaskCompleted, onTaskFailed, onTaskSkipped)
        {
            _taskToRun = taskToRun ?? throw new ArgumentNullException(nameof(taskToRun));
        }

        public SchedulerEngine(Func<Task> taskToRun, Action<DateTime> onTaskStarted = null,
            Action<DateTime> onTaskCompleted = null, Action<Exception, DateTime> onTaskFailed = null,
            Action<string, DateTime> onTaskSkipped = null)
            : this(onTaskStarted, onTaskCompleted, onTaskFailed, onTaskSkipped)
        {
            _asyncTaskToRun = taskToRun ?? throw new ArgumentNullException(nameof(taskToRun));
        }

        private SchedulerEngine(Action<DateTime> onTaskStarted, Action<DateTime> onTaskCompleted,
            Action<Exception, DateTime> onTaskFailed, Action<string, DateTime> onTaskSkipped)
        {
            _schedules = LoadSchedules().AsReadOnly();
            _scheduleCalculatorFactory = new ScheduleCalculatorFactory();
            _onTaskStarted = onTaskStarted ?? (_ => { });
            _onTaskCompleted = onTaskCompleted ?? (_ => { });
            _onTaskFailed = onTaskFailed ?? ((_, __) => { });
            _onTaskSkipped = onTaskSkipped ?? ((_, __) => { });
            _isDryRun = false;
        }
        #endregion

        #region "Public Methods"
        public void Start()
        {
            foreach (var schedule in _schedules)
            {
                if (schedule.Enabled)
                {
                    ScheduleTask(schedule);
                }
            }
        }

        public async Task StartAsync()
        {
            foreach (var schedule in _schedules)
            {
                if (schedule.Enabled)
                {
                    await ScheduleTaskAsync(schedule);
                }
            }
        }

        public void Stop()
        {
            while (_timers.TryTake(out var timer))
            {
                timer.Dispose();
            }
        }

        public void Pause() => Interlocked.Exchange(ref _isPaused, 1);
        public void Resume() => Interlocked.Exchange(ref _isPaused, 0);
        #endregion

        #region "Private Methods"

        #region "Load Schedules"
        private List<SchedulerSettings> LoadSchedules()
        {
            var config = SchedulerConfigSection.Instance;
            if (config?.Schedules == null)
                return new List<SchedulerSettings>();

            return config.Schedules
                .Cast<SchedulerSettingElement>()
                .Where(s => s.Enabled)
                .Select(s => new SchedulerSettings
                {
                    Enabled = s.Enabled,
                    ScheduleType = s.ScheduleType,
                    Hour = s.Hour,
                    Minute = s.Minute,
                    DayOfWeek = Enum.TryParse(s.DayOfWeek, true, out DayOfWeek result) ? result : DayOfWeek.Sunday,
                    DayOfMonth = s.DayOfMonth,
                    DryRun = s.DryRun
                }).ToList();
        }
        #endregion

        #region "Schedule Management"
        private void ScheduleTask(SchedulerSettings schedule)
        {
            if (!Enum.TryParse(schedule.ScheduleType, true, out ScheduleType scheduleType))
                return;

            _isDryRun = schedule.DryRun;

            if (scheduleType == ScheduleType.Immediate)
            {
                Task.Run(() => RunTaskSafe());
                return;
            }

            var delay = GetDelay(schedule, scheduleType);
            var interval = GetInterval(scheduleType);

            var timer = new System.Threading.Timer(_ => RunTaskSafe(), null, delay, interval);
            _timers.Add(timer);
        }

        private async Task ScheduleTaskAsync(SchedulerSettings schedule)
        {
            if (!Enum.TryParse(schedule.ScheduleType, true, out ScheduleType scheduleType))
                return;

            if (scheduleType == ScheduleType.Immediate)
            {
                await RunTaskSafeAsync().ConfigureAwait(false);
                return;
            }

            var delay = GetDelay(schedule, scheduleType);
            var interval = GetInterval(scheduleType);

            var timer = new System.Threading.Timer(async _ =>
            {
                try
                {
                    await RunTaskSafeAsync().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _onTaskFailed?.Invoke(ex, DateTime.Now);
                }
            }, null, delay, interval);

            _timers.Add(timer);
        }
        #endregion

        #region "Task Execution"
        private async Task RunTaskSafeAsync()
        {
            var now = DateTime.Now;

            if (IsPaused)
            {
                _onTaskSkipped?.Invoke("Task execution is paused.", now);
                return;
            }

            if (!_semaphore.Wait(0))
            {
                _onTaskSkipped?.Invoke("Task is already running. Skipping execution.", now);
                return;
            }

            try
            {
                Interlocked.Exchange(ref _isRunning, 1);
                _onTaskStarted?.Invoke(now);

                if (_isDryRun)
                {
                    _onTaskSkipped?.Invoke("Dry run mode: task execution skipped (simulated).", now);
                }
                else if (_asyncTaskToRun != null)
                {
                    await _retryHelper.RunWithRetryAsync(_asyncTaskToRun, now, _onTaskFailed, _onTaskSkipped)
                        .ConfigureAwait(false);
                }

                _onTaskCompleted?.Invoke(DateTime.Now);
            }
            catch (Exception ex)
            {
                _onTaskFailed?.Invoke(ex, DateTime.Now);
            }
            finally
            {
                Interlocked.Exchange(ref _isRunning, 0);
                _semaphore.Release();
            }
        }

        private void RunTaskSafe()
        {
            var now = DateTime.Now;

            if (IsPaused)
            {
                _onTaskSkipped?.Invoke("Task execution is paused.", now);
                return;
            }

            if (!_semaphore.Wait(0))
            {
                _onTaskSkipped?.Invoke("Task is already running. Skipping execution.", now);
                return;
            }

            try
            {
                Interlocked.Exchange(ref _isRunning, 1);
                _onTaskStarted?.Invoke(now);

                if (_isDryRun)
                {
                    _onTaskSkipped?.Invoke("Dry run mode: task execution skipped (simulated).", now);
                }
                else if (_taskToRun != null)
                {
                    _retryHelper.RunWithRetry(_taskToRun, now, _onTaskFailed, _onTaskSkipped);
                }

                _onTaskCompleted?.Invoke(DateTime.Now);
            }
            catch (Exception ex)
            {
                _onTaskFailed?.Invoke(ex, DateTime.Now);
            }
            finally
            {
                Interlocked.Exchange(ref _isRunning, 0);
                _semaphore.Release();
            }
        }
        #endregion

        #region "Schedule Helpers"
        private TimeSpan GetDelay(SchedulerSettings schedule, ScheduleType scheduleType)
        {
            DateTime nextRun = _scheduleCalculatorFactory
                .GetCalculator(scheduleType)
                .GetNextOccurrence(DateTime.Now, schedule);

            TimeSpan delay = nextRun - DateTime.Now;
            return delay > TimeSpan.Zero ? delay : TimeSpan.Zero;
        }
        private static TimeSpan GetInterval(ScheduleType scheduleType)
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
        #endregion

        #endregion

        #region "Dispose Pattern"
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                Stop();
                _semaphore.Dispose();
            }

            _disposed = true;
        }

        ~SchedulerEngine()
        {
            Dispose(false);
        }
        #endregion
    }
}