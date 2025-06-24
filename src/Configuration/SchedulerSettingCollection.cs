using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomateCore.Configuration
{
    [ConfigurationCollection(typeof(SchedulerSettingElement), AddItemName = "add")]
    public class SchedulerSettingCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement() => new SchedulerSettingElement();
        protected override object GetElementKey(ConfigurationElement element) => ((SchedulerSettingElement)element).ScheduleType;

        public SchedulerSettingElement this[int index] => (SchedulerSettingElement)BaseGet(index);
    }
}
