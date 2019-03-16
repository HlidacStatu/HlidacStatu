using DotNet.Highcharts.Attributes;

namespace DotNet.Highcharts.Options.ParallelAxes
{
    /// <summary>
    /// Event handlers for the axis.
    /// </summary>
    public class ParallelAxesEvents
    {
        /// <summary>
        /// An event fired after the breaks have rendered.
        /// Default: undefined
        /// </summary>
        [JsonFormatter("{0}")]
        public string AfterBreaks { get; set; }

        /// <summary>
        /// As opposed to the setExtremes event, this event fires after the final min and max values are computed and corrected for minRange.
        /// Fires when the minimum and maximum is set for the axis, either by calling the .setExtremes() method or by selecting an area in the chart. One parameter, event, is passed to the function, containing common event information.
        /// The new user set minimum and maximum values can be found by event.min and event.max. These reflect the axis minimum and maximum in axis values. The actual data extremes are found in event.dataMin and event.dataMax.
        /// Default: undefined
        /// </summary>
        [JsonFormatter("{0}")]
        public string AfterSetExtremes { get; set; }

        /// <summary>
        /// An event fired when a break from this axis occurs on a point.
        /// Default: undefined
        /// </summary>
        [JsonFormatter("{0}")]
        public string PointBreak { get; set; }

        /// <summary>
        /// An event fired when a point falls inside a break from this axis.
        /// Default: undefined
        /// </summary>
        [JsonFormatter("{0}")]
        public string PointInBreak { get; set; }

        /// <summary>
        /// Fires when the minimum and maximum is set for the axis, either by calling the .setExtremes() method or by selecting an area in the chart. One parameter, event, is passed to the function, containing common event information.
        /// The new user set minimum and maximum values can be found by event.min and event.max. These reflect the axis minimum and maximum in data values. When an axis is zoomed all the way out from the "Reset zoom" button, event.min and event.max are null, and the new extremes are set based on this.dataMin and this.dataMax.
        /// Default: undefined
        /// </summary>
        [JsonFormatter("{0}")]
        public string SetExtremes { get; set; }
    }
}
