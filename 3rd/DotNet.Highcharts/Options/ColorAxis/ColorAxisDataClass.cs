using DotNet.Highcharts.Helpers;
using System.Drawing;

namespace DotNet.Highcharts.Options.ColorAxis
{
    /// <summary>
    /// An array of data classes or ranges for the choropleth map. If none given, the color axis is scalar and values are distributed as a gradient between the minimum and maximum colors.
    /// </summary>
    public class ColorAxisDataClass
    {
        /// <summary>
        /// The color of each data class. If not set, the color is pulled from the global or chart-specific colors array. In styled mode, this option is ignored. Instead, use colors defined in CSS.
        /// Default: undefined
        /// </summary>
        public Color? Color { get; set; }

        /// <summary>
        /// The start of the value range that the data class represents, relating to the point value.
        /// The range of each dataClass is closed in both ends, but can be overridden by the next dataClass.
        /// Default: undefined
        /// </summary>
        public Number? From { get; set; }

        /// <summary>
        /// The name of the data class as it appears in the legend. If no name is given, it is automatically created based on the from and to values. For full programmatic control, legend.labelFormatter can be used. In the formatter, this.from and this.to can be accessed.
        /// Default: undefined
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The end of the value range that the data class represents, relating to the point value.
        /// The range of each dataClass is closed in both ends, but can be overridden by the next dataClass.
        /// Default: undefined
        /// </summary>
        public Number? To { get; set; }
    }
}
