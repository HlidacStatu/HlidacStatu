using System;

namespace DotNet.Highcharts.Helpers
{
    /// <summary>
    /// Helper tools for the library
    /// </summary>
    public static class Tools
    {
        /// <summary>
        /// Returns the number of milliseconds since Jan 1, 1970. This is also UNIX timestamps which JavaScript uses.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static double GetTotalMilliseconds(DateTime dateTime)
        {
            DateTime d1 = new DateTime(1970, 1, 1);
            DateTime d2 = dateTime.ToUniversalTime();
            TimeSpan ts = new TimeSpan(d2.Ticks - d1.Ticks);

            return ts.TotalMilliseconds;
        }
    }
}