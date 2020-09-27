using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HlidacStatu.Lib.Data;

namespace HlidacStatu.Lib.Watchdogs
{
    public class Tools
    {
        public static DateTime RoundWatchdogTime(WatchDog.PeriodTime period, DateTime dt)
        {
            switch (period)
            {
                case WatchDog.PeriodTime.Immediatelly:
                    return dt;
                case WatchDog.PeriodTime.Hourly:
                    return dt.AddMinutes(-1 * dt.Minute);
                case WatchDog.PeriodTime.Daily:
                case WatchDog.PeriodTime.Monthly:
                    return dt.Date;
                case WatchDog.PeriodTime.Weekly:
                    return dt.AddHours(-1 * dt.Hour);
                default:
                    return dt;
            }

        }

        public static bool ReadyToRun(WatchDog.PeriodTime period, DateTime? lastRun, DateTime? newRun )
        {
            newRun = newRun ?? DateTime.Now;
            if (lastRun.HasValue == false)
                return true;

            //round lastsearch to the begin of the day

            var lastSearch = lastRun.Value;
            lastSearch = RoundWatchdogTime(period, lastSearch);
            switch (period)
            {
                case WatchDog.PeriodTime.Immediatelly:
                    return true;
                case WatchDog.PeriodTime.Hourly:
                    return (newRun.Value - lastSearch).TotalHours >= 1.0;
                case WatchDog.PeriodTime.Daily:
                    return (newRun.Value - lastSearch).TotalHours >= 23.0;
                case WatchDog.PeriodTime.Weekly:
                    return (newRun.Value - lastSearch).TotalDays >= 6.5;
                case WatchDog.PeriodTime.Monthly:
                    return Devmasters.DT.DateTimeSpan.CompareDates(newRun.Value, lastRun.Value).Months > 0;
            }
            return true;
        }

    }
}
