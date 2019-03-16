using System.Drawing;
using DotNet.Highcharts.Helpers;

namespace DotNet.Highcharts.Options.Accessibility
{
    public class FocusBorderStyle
    {
        /// <summary>
        /// Border radius of the focus border.
        /// Default: 3
        /// </summary>
        public Number? BorderRadius { get; set; }

        /// <summary>
        /// Color of the focus border.
        /// Defaults to #000000.
        /// </summary>
        public Color? Color { get; set; }

        /// <summary>
        /// Line width of the focus border.
        /// Default: 2
        /// </summary>
        public Number? LineWidth { get; set; }
    }
}
