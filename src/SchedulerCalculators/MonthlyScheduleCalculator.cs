using AutomateCore.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomateCore.SchedulerCalculators
{
    public class MonthlyScheduleCalculator : IScheduleCalculator
    {
        public DateTime GetNextOccurrence(DateTime now, SchedulerSettings config)
        {
            ValidateConfiguration(config);

            // Calculate initial next run date
            DateTime nextRun = CalculateInitialRunDate(now, config);

            // Adjust if the calculated time is in the past
            if (now > nextRun)
            {
                nextRun = CalculateNextMonthRunDate(nextRun, config);
            }

            return nextRun;
        }

        private static void ValidateConfiguration(SchedulerSettings config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            if (config.DayOfMonth < 1 || config.DayOfMonth > 31)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(config.DayOfMonth),
                    config.DayOfMonth,
                    "Day of month must be between 1-31");
            }

            if (config.Hour < 0 || config.Hour > 23)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(config.Hour),
                    config.Hour,
                    "Hour must be between 0-23");
            }

            if (config.Minute < 0 || config.Minute > 59)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(config.Minute),
                    config.Minute,
                    "Minutes must be between 0-59");
            }
        }

        private static DateTime CalculateInitialRunDate(DateTime now, SchedulerSettings config)
        {
            if (now.Year < 1 || now.Year > 9999)
                throw new ArgumentOutOfRangeException(nameof(now), "Year must be between 1-9999");
            if (now.Month < 1 || now.Month > 12)
                throw new ArgumentOutOfRangeException(nameof(now), "Month must be between 1-12");

            int daysInMonth = DateTime.DaysInMonth(now.Year, now.Month);
            int day = Clamp(config.DayOfMonth, 1, daysInMonth);

            try
            {
                return new DateTime(now.Year, now.Month, day, config.Hour, config.Minute, 0, DateTimeKind.Unspecified);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                throw new InvalidOperationException("Failed to create valid DateTime with current configuration", ex);
            }
        }

        private static DateTime CalculateNextMonthRunDate(DateTime currentRun, SchedulerSettings config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            int year = currentRun.Month == 12 ? currentRun.Year + 1 : currentRun.Year;
            int month = currentRun.Month == 12 ? 1 : currentRun.Month + 1;

            int daysInMonth = DateTime.DaysInMonth(year, month);
            int safeDay = Clamp(config.DayOfMonth, 1, daysInMonth);

            try
            {
                return new DateTime(
                    year,
                    month,
                    safeDay,
                    config.Hour,
                    config.Minute,
                    0,
                    currentRun.Kind
                );
            }
            catch (ArgumentOutOfRangeException ex)
            {
                throw new InvalidOperationException(
                    $"The requested day {config.DayOfMonth} doesn't exist in {year}-{month:00}. " +
                    $"Maximum days in month: {daysInMonth}", ex);
            }
        }

        // Custom Clamp for .NET Framework 4.8
        private static int Clamp(int value, int min, int max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
    }
}
