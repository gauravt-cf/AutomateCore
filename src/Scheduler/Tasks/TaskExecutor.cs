using AutomateCore.Scheduler.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutomateCore.Scheduler.Tasks
{
    public class TaskExecutor : ITaskExecutor
    {
        public string TaskName { get; }
        public bool IsRunning { get; set; }
        private readonly Action<CancellationToken> _syncAction;
        private readonly Func<CancellationToken, Task> _asyncAction;

        public TaskExecutor(string taskName, Action<CancellationToken> action)
        {
            TaskName = taskName;
            _syncAction = action;
        }

        public TaskExecutor(string taskName, Func<CancellationToken, Task> action)
        {
            TaskName = taskName;
            _asyncAction = action;
        }

        public void Execute(CancellationToken cancellationToken)
        {
            _syncAction?.Invoke(cancellationToken);
        }

        public Task ExecuteAsync(CancellationToken cancellationToken)
        {
            return _asyncAction != null
                ? _asyncAction(cancellationToken)
                : Task.Run(() => _syncAction?.Invoke(cancellationToken));
        }
    }
}
