using AutomateCore.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomateCore.SchedulerCalculators
{
    public class ImmediateScheduleCalculator : IScheduleCalculator
    {
        public DateTime GetNextOccurrence(DateTime now, SchedulerSettings config)
        {
            return DateTime.MaxValue;
        }
    }
}
