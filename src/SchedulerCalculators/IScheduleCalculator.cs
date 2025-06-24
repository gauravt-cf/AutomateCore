using AutomateCore.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomateCore.SchedulerCalculators
{
    public interface IScheduleCalculator
    {
        DateTime GetNextOccurrence(DateTime now, SchedulerSettings config);
    }
}
