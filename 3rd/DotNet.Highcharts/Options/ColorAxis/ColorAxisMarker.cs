using DotNet.Highcharts.Helpers;
using System.Drawing;

namespace DotNet.Highcharts.Options.ColorAxis
{
    /// <summary>
    /// The triangular marker on a scalar color axis that points to the value of the hovered area. To disable the marker, set marker: null.
    /// </summary>
    public class ColorAxisMarker
    {
        /// <summary>
        /// Animation for the marker as it moves between values. Set to false to disable animation. Defaults to { duration: 50 }.
        /// </summary>
        public ColorAxisMarkerAnimation Animation { get; set; }

        /// <summary>
        /// The color of the marker.
        /// Default: #999999
        /// </summary>
        public Color? Color { get; set; }
    }

    /// <summary>
    /// Animation for the marker as it moves between values. Set to false to disable animation. Defaults to { duration: 50 }.
    /// </summary>
    public class ColorAxisMarkerAnimation
    {
        /// <summary>
        /// Defaults to 50
        /// </summary>
        public Number? Duration { get; set; }
    }
}
