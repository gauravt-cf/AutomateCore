using AutomateCore.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomateCore.SchedulerCalculators
{
    public class HourlyScheduleCalculator : IScheduleCalculator
    {
        public DateTime GetNextOccurrence(DateTime now, SchedulerSettings config)
        {
            // Round down to the start of current hour
            var currentHour = now.Date.AddHours(now.Hour);

            // If current time is exactly on the hour, return next hour
            if (now == currentHour)
                return currentHour.AddHours(1);

            // Otherwise return start of next hour
            return currentHour.AddHours(1);
        }
    }
}
