using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomateCore.Configuration
{
    public class SchedulerSettingElement : ConfigurationElement
    {
        [ConfigurationProperty("enabled", DefaultValue = false, IsRequired = true)]
        public bool Enabled => (bool)this["enabled"];

        [ConfigurationProperty("scheduleType", IsRequired = true, DefaultValue = "Daily")]
        public string ScheduleType => (string)this["scheduleType"];

        [ConfigurationProperty("hour", IsRequired = false, DefaultValue = 0)]
        public int Hour => (int)this["hour"];

        [ConfigurationProperty("minute", IsRequired = false, DefaultValue = 0)]
        public int Minute => (int)this["minute"];

        [ConfigurationProperty("dayOfWeek", IsRequired = false, DefaultValue = "Sunday")]
        public string DayOfWeek => (string)this["dayOfWeek"];

        [ConfigurationProperty("dayOfMonth", IsRequired = false, DefaultValue = 1)]
        public int DayOfMonth => (int)this["dayOfMonth"];
        [ConfigurationProperty("dryRun", IsRequired = false, DefaultValue = false)]
        public bool DryRun => (bool)this["dryRun"];
        [ConfigurationProperty("maxConcurrent", IsRequired = false, DefaultValue = 1)]
        public int MaxConcurrentExecutions => (int)this["maxConcurrent"];
        [ConfigurationProperty("taskName", IsRequired = false, DefaultValue = "")]
        public string TaskName => (string)this["taskName"];
        [ConfigurationProperty("isPaused", IsRequired = false, DefaultValue = false)]
        public bool IsPaused => (bool)this["isPaused"];
    }
}
