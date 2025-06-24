using AutomateCore.Enums;
using AutomateCore.Scheduler.Interfaces;
using AutomateCore.SchedulerCalculators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomateCore.Scheduler
{
    public class ScheduleCalculatorFactory : IScheduleCalculatorFactory
    {
        public IScheduleCalculator GetCalculator(ScheduleType scheduleType)
        {
            switch (scheduleType)
            {
                case ScheduleType.Immediate:
                    return new ImmediateScheduleCalculator();
                case ScheduleType.Hourly:
                    return new HourlyScheduleCalculator();
                case ScheduleType.Daily:
                    return new DailyScheduleCalculator();
                case ScheduleType.Weekly:
                    return new WeeklyScheduleCalculator();
                case ScheduleType.Monthly:
                    return new MonthlyScheduleCalculator();
                case ScheduleType.Every2Minutes:
                case ScheduleType.Every5Minutes:
                case ScheduleType.Every10Minutes:
                case ScheduleType.Every15Minutes:
                case ScheduleType.Every30Minutes:
                case ScheduleType.Every2Hours:
                case ScheduleType.Every4Hours:
                case ScheduleType.Every6Hours:
                case ScheduleType.EveryMinute:
                    return new CommonScheduleCalculator();
                default:
                    throw new ArgumentOutOfRangeException(nameof(scheduleType));
            }
        }
    }
}
