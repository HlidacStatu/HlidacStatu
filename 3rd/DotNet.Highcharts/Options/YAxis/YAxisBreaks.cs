using DotNet.Highcharts.Helpers;

namespace DotNet.Highcharts.Options.YAxis
{
    /// <summary>
    /// An array defining breaks in the axis, the sections defined will be left out and all the points shifted closer to each other.
    /// </summary>
    public class YAxisBreaks
    {
        /// <summary>
        /// A number indicating how much space should be left between the start and the end of the break. The break size is given in axis units, so for instance on a datetime axis, a break size of 3600000 would indicate the equivalent of an hour.
        /// Default: 0
        /// </summary>
        public Number? BreakSize { get; set; }

        /// <summary>
        /// The point where the break starts.
        /// Default: undefined
        /// </summary>
        public Number? From { get; set; }

        /// <summary>
        /// Defines an interval after which the break appears again. By default the breaks do not repeat.
        /// Default: 0
        /// </summary>
        public Number? Repeat { get; set; }

        /// <summary>
        /// The point where the break ends.
        /// Default: undefined
        /// </summary>
        public Number? To { get; set; }
    }
}
