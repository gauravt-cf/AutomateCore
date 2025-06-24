using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomateCore.Configuration
{
    #region Configuration Classes
    public class SchedulerConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("", IsDefaultCollection = true)]
        public SchedulerSettingCollection Schedules => (SchedulerSettingCollection)base[""];

        [ConfigurationProperty("tasks", IsDefaultCollection = false)]
        public TaskCollection Tasks => (TaskCollection)base["tasks"];

        public static SchedulerConfigSection Instance =>
                (SchedulerConfigSection)ConfigurationManager.GetSection("schedulerSettings") ?? new SchedulerConfigSection();
    }
    #endregion
}
