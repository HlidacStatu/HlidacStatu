using DotNet.Highcharts.Helpers;

namespace DotNet.Highcharts.Options
{
    /// <summary>
    /// Options for a scrollable plot area. This feature provides a 
    /// minimum width for the plot area of the chart. If the width 
    /// gets smaller than this, typically on mobile devices, a native 
    /// browser scrollbar is presented below the chart. This scrollbar 
    /// provides smooth scrolling for the contents of the plot area, 
    /// whereas the title, legend and axes are fixed.
    /// </summary>
    public class ScrollablePlotArea
    {
        /// <summary>
        /// The minimum width for the plot area. If it gets smaller than 
        /// this, the plot area will become scrollable.
        /// Default: undefined
        /// </summary>
        public Number? MinWidth { get; set; }

        /// <summary>
        /// The initial scrolling position of the scrollable plot area. 
        /// Ranges from 0 to 1, where 0 aligns the plot area to the left 
        /// and 1 aligns it to the right. Typically we would use 1 if the 
        /// chart has right aligned Y axes.
        /// Default: undefined
        /// </summary>
        public Number? ScrollPositionX { get; set; }
    }
}
