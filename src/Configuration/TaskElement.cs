using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomateCore.Configuration
{
    public class TaskElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name => (string)this["name"];

        [ConfigurationProperty("scheduleType", IsRequired = true)]
        public string ScheduleType => (string)this["scheduleType"];

        [ConfigurationProperty("enabled", DefaultValue = true)]
        public bool Enabled => (bool)this["enabled"];

        [ConfigurationProperty("hour", DefaultValue = 0)]
        public int Hour => (int)this["hour"];

        [ConfigurationProperty("minute", DefaultValue = 0)]
        public int Minute => (int)this["minute"];

        [ConfigurationProperty("dayOfWeek", DefaultValue = "")]
        public string DayOfWeek => (string)this["dayOfWeek"];

        [ConfigurationProperty("dayOfMonth", DefaultValue = 0)]
        public int DayOfMonth => (int)this["dayOfMonth"];

        [ConfigurationProperty("dryRun", DefaultValue = false)]
        public bool DryRun => (bool)this["dryRun"];
        [ConfigurationProperty("maxConcurrent", IsRequired = false, DefaultValue = 1)]
        public int MaxConcurrentExecutions => (int)this["maxConcurrent"];
        [ConfigurationProperty("paused", IsRequired = false, DefaultValue = true)]
        public bool paused => (bool)this["paused"];
    }
    public class TaskCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement() => new TaskElement();
        protected override object GetElementKey(ConfigurationElement element) => ((TaskElement)element).Name;
    }
}
