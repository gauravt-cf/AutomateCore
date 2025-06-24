using AutomateCore.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomateCore.SchedulerCalculators
{
    public class DailyScheduleCalculator : IScheduleCalculator
    {
        public DateTime GetNextOccurrence(DateTime now, SchedulerSettings config)
        {
            var todayAtTime = now.Date.Add(new TimeSpan(config.Hour, config.Minute, 0));
            return now < todayAtTime ? todayAtTime : todayAtTime.AddDays(1);
        }
    }
}
