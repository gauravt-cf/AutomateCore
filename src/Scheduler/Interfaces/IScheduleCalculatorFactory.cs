using AutomateCore.Enums;
using AutomateCore.SchedulerCalculators;

namespace AutomateCore.Scheduler.Interfaces
{
    public interface IScheduleCalculatorFactory
    {
        IScheduleCalculator GetCalculator(ScheduleType scheduleType);
    }
}
