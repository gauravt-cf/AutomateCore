using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomateCore.Enums
{
    public enum ScheduleType
    {
        /// <summary>Run immediately (once)</summary>
        [Description("Run immediately (once)")]
        Immediate,

        /// <summary>Run every minute</summary>
        [Description("Run every minute")]
        EveryMinute,

        /// <summary>Run every 2 minutes</summary>
        [Description("Run every 2 minutes")]
        Every2Minutes,

        /// <summary>Run every 5 minutes</summary>
        [Description("Run every 5 minutes")]
        Every5Minutes,

        /// <summary>Run every 10 minutes</summary>
        [Description("Run every 10 minutes")]
        Every10Minutes,

        /// <summary>Run every 15 minutes</summary>
        [Description("Run every 15 minutes")]
        Every15Minutes,

        /// <summary>Run every 30 minutes</summary>
        [Description("Run every 30 minutes")]
        Every30Minutes,

        /// <summary>Run every hour</summary>
        [Description("Run every hour")]
        Hourly,

        /// <summary>Run every 2 hours</summary>
        [Description("Run every 2 hours")]
        Every2Hours,

        /// <summary>Run every 4 hours</summary>
        [Description("Run every 4 hours")]
        Every4Hours,

        /// <summary>Run every 6 hours</summary>
        [Description("Run every 6 hours")]
        Every6Hours,

        /// <summary>Run twice a day</summary>
        [Description("Run twice a day")]
        TwiceDaily,

        /// <summary>Run once daily</summary>
        [Description("Run once daily")]
        Daily,

        /// <summary>Run every weekday (Monday to Friday)</summary>
        [Description("Run every weekday (Mon–Fri)")]
        Weekdays,

        /// <summary>Run every weekend (Saturday and Sunday)</summary>
        [Description("Run on weekends (Sat–Sun)")]
        Weekends,

        /// <summary>Run every Monday</summary>
        [Description("Run every Monday")]
        Weekly_Monday,

        /// <summary>Run every Friday</summary>
        [Description("Run every Friday")]
        Weekly_Friday,

        /// <summary>Run once a week</summary>
        [Description("Run once a week")]
        Weekly,

        /// <summary>Run every two weeks</summary>
        [Description("Run every 2 weeks")]
        [Obsolete("No Yet Implemented")]
        BiWeekly,

        /// <summary>Run twice a month (e.g., 1st and 15th)</summary>
        [Description("Run twice a month")]
        SemiMonthly,

        /// <summary>Run once a month</summary>
        [Description("Run once a month")]
        Monthly,

        /// <summary>Run on the first day of the month</summary>
        [Description("Run on the first day of the month")]
        [Obsolete("No Yet Implemented")]
        Monthly_FirstDay,

        /// <summary>Run on the last day of the month</summary>
        [Description("Run on the last day of the month")]
        [Obsolete("No Yet Implemented")]
        Monthly_LastDay,

        /// <summary>Run every 3 months</summary>
        [Description("Run every 3 months")]
        Quarterly,

        /// <summary>Run twice a year</summary>
        [Description("Run twice a year")]
        SemiAnnually,

        /// <summary>Run once a year</summary>
        [Description("Run once a year")]
        [Obsolete("No Yet Implemented")]
        Yearly,

        /// <summary>Run once when the application or service starts</summary>
        [Description("Run once when the application or service starts")]
        [Obsolete("No Yet Implemented")]
        OnStartup,

        /// <summary>Run only when manually triggered</summary>
        //[Description("Run only when manually triggered")]
        //Manual
    }
}
