using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomateCore.Scheduler
{
    public interface ISchedulerEngine
    {
        void Start();
        Task StartAsync();
        void Stop();
        void Pause();
        void Resume();
        bool IsRunning { get; }
    }
}
