using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomateCore.Configuration
{
    public class SchedulerSettings
    {
        public bool Enabled { get; set; }
        public string ScheduleType { get; set; }
        public int Hour { get; set; }
        public int Minute { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public int DayOfMonth { get; set; }
        public bool DryRun { get; set; }
        public bool IsPaused { get; set; }
        public string TaskName { get; set; }
        public int MaxConcurrentExecutions { get; set; } = 1;
        public static SchedulerSettings LoadFromConfig()
        {
            var config = SchedulerConfigSection.Instance;

            if (config?.Schedules == null || config.Schedules.Count == 0)
                return null;

            var firstSchedule = config.Schedules[0];

            return new SchedulerSettings
            {
                Enabled = firstSchedule.Enabled,
                ScheduleType = firstSchedule.ScheduleType,
                Hour = firstSchedule.Hour,
                Minute = firstSchedule.Minute,
                DayOfWeek = Enum.TryParse(firstSchedule.DayOfWeek, true, out DayOfWeek result) ? result : DayOfWeek.Sunday,
                DayOfMonth = firstSchedule.DayOfMonth,
                DryRun = firstSchedule.DryRun
            };
        }
        public static List<SchedulerSettings> LoadListFromConfig()
        {
            var config = SchedulerConfigSection.Instance;

            if (config?.Schedules == null || config.Schedules.Count == 0)
                return null;

            var settingsList = new List<SchedulerSettings>();
            foreach (SchedulerSettingElement schedule in config.Schedules)
            {
                settingsList.Add(new SchedulerSettings
                {
                    Enabled = schedule.Enabled,
                    ScheduleType = schedule.ScheduleType,
                    Hour = schedule.Hour,
                    Minute = schedule.Minute,
                    DayOfWeek = Enum.TryParse(schedule.DayOfWeek, true, out DayOfWeek result) ? result : DayOfWeek.Sunday,
                    DayOfMonth = schedule.DayOfMonth,
                    DryRun = schedule.DryRun,
                    IsPaused = schedule.IsPaused,
                    MaxConcurrentExecutions = schedule.MaxConcurrentExecutions,
                    TaskName = schedule.TaskName
                });
            }
            return settingsList;
        }
    }
}
