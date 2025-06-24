using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomateCore.Configuration
{
    public class SchedulerTaskConfig
    {
        public bool Enabled { get; set; }
        public string ScheduleType { get; set; }
        public int Hour { get; set; }
        public int Minute { get; set; }
        public string DayOfWeek { get; set; }
        public int DayOfMonth { get; set; }
        public bool DryRun { get; set; }
        public bool Paused { get; set; }
        public string TaskName { get; set; }
        public int MaxConcurrentExecutions { get; set; } = 1;
    }
}
