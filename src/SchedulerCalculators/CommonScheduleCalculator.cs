using AutomateCore.Configuration;
using AutomateCore.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomateCore.SchedulerCalculators
{
    public class CommonScheduleCalculator : IScheduleCalculator
    {
        public DateTime GetNextOccurrence(DateTime now, SchedulerSettings config)
        {
            ScheduleType scheduleType = GetScheduleType(config.ScheduleType);
            switch (scheduleType)
            {
                case ScheduleType.Every2Minutes:
                    return now.AddMinutes(2);
                case ScheduleType.Every5Minutes:
                    return now.AddMinutes(5);
                case ScheduleType.Every10Minutes:
                    return now.AddMinutes(10);
                case ScheduleType.Every15Minutes:
                    return now.AddMinutes(15);
                case ScheduleType.Every30Minutes:
                    return now.AddMinutes(30);
                case ScheduleType.Every2Hours:
                    return Every2Hours(now);
                case ScheduleType.Every4Hours:
                    return Every4Hours(now);
                case ScheduleType.Every6Hours:
                    return Every6Hours(now);
                case ScheduleType.EveryMinute:
                    return now.AddMinutes(1);
                case ScheduleType.Quarterly:
                    return NextQuarterlyOccurrence(now);
                case ScheduleType.TwiceDaily:
                    return GetNextTwiceDaily(now);
                case ScheduleType.Weekends:
                    return GetNextWeekend(now);
                case ScheduleType.Weekly_Friday:
                    return GetNextWeeklyDay(now, DayOfWeek.Friday);
                case ScheduleType.Weekly_Monday:
                    return GetNextWeeklyDay(now, DayOfWeek.Friday);
                case ScheduleType.SemiMonthly:
                    config.Hour = config.Hour > 0 ? config.Hour : 9;
                    return GetNextSemiMonthly(now, config.Hour);
                case ScheduleType.SemiAnnually:
                    config.Hour = config.Hour > 0 ? config.Hour : 9;
                    return GetNextSemiAnnually(now, config.Hour);
                default:
                    throw new InvalidOperationException($"Unsupported ScheduleType: {scheduleType}");
            }
        }

        private ScheduleType GetScheduleType(string schduleType)
        {
            if (!string.IsNullOrEmpty(schduleType) && Enum.TryParse(schduleType, out ScheduleType schedule)) return schedule;
            return ScheduleType.Daily;
        }


        #region "SCHEDULERS"
        private DateTime GetNextSemiAnnually(DateTime now, int hour = 9)
        {
            var year = now.Year;
            var jan1 = new DateTime(year, 1, 1, hour, 0, 0);
            var jul1 = new DateTime(year, 7, 1, hour, 0, 0);

            if (now < jan1)
                return jan1;
            else if (now < jul1)
                return jul1;
            else
                return new DateTime(year + 1, 1, 1, 9, 0, 0); // Next Jan 1 of next year
        }
        private DateTime GetNextSemiMonthly(DateTime now, int hour)
        {
            var day = now.Day;
            var timeOfDay = new TimeSpan(hour, 0, 0); // Or make this configurable (default 9 AM)

            if (day < 15)
            {
                // Schedule for the 15th of the current month
                return new DateTime(now.Year, now.Month, 15).Add(timeOfDay);
            }
            else
            {
                // Schedule for the 1st of the next month
                var nextMonth = now.AddMonths(1);
                return new DateTime(nextMonth.Year, nextMonth.Month, 1).Add(timeOfDay);
            }
        }

        private DateTime GetNextWeeklyDay(DateTime now, DayOfWeek targetDay)
        {
            // Set scheduled time (e.g., 9:00 AM)
            TimeSpan runTime = new TimeSpan(9, 0, 0); // Adjust as needed

            int daysUntilTarget = ((int)targetDay - (int)now.DayOfWeek + 7) % 7;
            if (daysUntilTarget == 0 && now.TimeOfDay >= runTime)
            {
                // Already past run time for today, schedule for next week
                daysUntilTarget = 7;
            }

            return now.Date.AddDays(daysUntilTarget).Add(runTime);
        }

        private DateTime GetNextWeekend(DateTime now)
        {
            // Define the target execution time (e.g., 9:00 AM)
            TimeSpan runTime = new TimeSpan(9, 0, 0); // 9:00 AM

            DateTime next = now.Date;

            for (int i = 0; i <= 7; i++)
            {
                next = now.Date.AddDays(i);

                if (next.DayOfWeek == DayOfWeek.Saturday || next.DayOfWeek == DayOfWeek.Sunday)
                {
                    DateTime scheduledTime = next.Add(runTime);
                    if (scheduledTime > now)
                        return scheduledTime;
                }
            }

            // fallback just in case (though the loop ensures this isn't reached)
            return now.AddDays(1).Date.Add(runTime);
        }
        private DateTime GetNextTwiceDaily(DateTime now)
        {
            var todayMorning = new DateTime(now.Year, now.Month, now.Day, 9, 0, 0);  // 9:00 AM
            var todayEvening = new DateTime(now.Year, now.Month, now.Day, 21, 0, 0); // 9:00 PM

            if (now < todayMorning)
                return todayMorning;
            else if (now < todayEvening)
                return todayEvening;
            else
                return todayMorning.AddDays(1); // Next day 9:00 AM
        }
        private DateTime Every2Hours(DateTime now)
        {
            var next = now.AddHours(1);
            int nextHour = ((next.Hour / 2) + 1) * 2;
            if (nextHour >= 24)
            {
                return next.Date.AddDays(1).AddHours(0);
            }
            return next.Date.AddHours(nextHour);
        }
        private DateTime Every4Hours(DateTime now)
        {
            var next = now.AddHours(1);

            // Find the next multiple of 4 (e.g., 0, 4, 8, 12, 16, 20)
            int nextHour = ((next.Hour / 4) + 1) * 4;

            if (nextHour >= 24)
            {
                // Wrap to next day at midnight
                return next.Date.AddDays(1).AddHours(0);
            }

            return next.Date.AddHours(nextHour);
        }
        private DateTime Every6Hours(DateTime now)
        {
            var next = now.AddHours(1);
            int nextHours = ((next.Hour / 6) + 1) * 6;
            if (nextHours >= 24)
            {
                return next.Date.AddDays(1).AddHours(0);
            }
            return next.Date.AddHours(nextHours);
        }
        private DateTime NextQuarterlyOccurrence(DateTime now)
        {
            int currentQuarter = (now.Month - 1) / 3 + 1;
            int nextQuarterMonth = (currentQuarter * 3) + 1;

            int year = now.Year;
            if (nextQuarterMonth > 12)
            {
                nextQuarterMonth = 1;
                year += 1;
            }

            return new DateTime(year, nextQuarterMonth, 1, 0, 0, 0);
        }
        #endregion
    }
}
