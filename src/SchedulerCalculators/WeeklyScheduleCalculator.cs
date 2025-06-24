using AutomateCore.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomateCore.SchedulerCalculators
{
    public class WeeklyScheduleCalculator : IScheduleCalculator
    {
        public DateTime GetNextOccurrence(DateTime now, SchedulerSettings config)
        {
            // Validate configuration
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            if (config.Hour < 0 || config.Hour > 23)
                throw new ArgumentOutOfRangeException(nameof(config.Hour), "Hour must be between 0-23");

            if (config.Minute < 0 || config.Minute > 59)
                throw new ArgumentOutOfRangeException(nameof(config.Minute), "Minute must be between 0-59");


            var daysUntilNext = (config.DayOfWeek - now.DayOfWeek + 7) % 7;
            daysUntilNext = daysUntilNext == 0 && now.TimeOfDay >= new TimeSpan(config.Hour, config.Minute, 0)
                ? 7
                : daysUntilNext;

            var nextRun = now.Date
                .AddDays(daysUntilNext)
                .AddHours(config.Hour)
                .AddMinutes(config.Minute);

            return nextRun;
        }
    }
}
