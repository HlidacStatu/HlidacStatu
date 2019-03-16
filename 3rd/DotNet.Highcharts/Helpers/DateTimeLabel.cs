using DotNet.Highcharts.Attributes;

namespace DotNet.Highcharts.Helpers
{
    public class DateTimeLabel
    {
        /// <summary>
        /// Format of the millisecond at the axis label.
        /// </summary>
        public string Millisecond { get; set; }
        /// <summary>
        /// Format of the second at the axis label.
        /// </summary>
        public string Second { get; set; }

        /// <summary>
        /// Format of the minute at the axis label.
        /// </summary>
        public string Minute { get; set; }

        /// <summary>
        /// Format of the hour at the axis label.
        /// </summary>
        public string Hour { get; set; }

        /// <summary>
        /// Format of the day at the axis label.
        /// </summary>
        public string Day { get; set; }

        /// <summary>
        /// Format of the week at the axis label.
        /// </summary>
        public string Week { get; set; }

        /// <summary>
        /// Format of the month at the axis label.
        /// </summary>
        public string Month { get; set; }

        /// <summary>
        /// Format of the seconds at the axis label.
        /// </summary>
        public string Year { get; set; }
    }
}