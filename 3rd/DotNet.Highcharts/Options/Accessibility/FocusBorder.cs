using DotNet.Highcharts.Helpers;

namespace DotNet.Highcharts.Options.Accessibility
{
    /// <summary>
    /// Options for the focus border drawn around elements while navigating through them.
    /// </summary>
    public class FocusBorder
    {
        /// <summary>
        /// Enable/disable focus border for chart.
        /// Default: true
        /// </summary>
        public bool? Enabled { get; set; }

        /// <summary>
        /// Hide the browser's default focus indicator.
        /// Default: true
        /// </summary>
        public bool? HideBrowserFocusIcon { get; set; }

        /// <summary>
        /// Focus border margin around the elements.
        /// Defaults to 2.
        /// </summary>
        public Number? Margin { get; set; }

        /// <summary>
        /// Style options for the focus border drawn around elements while navigating through them. Note that some browsers in addition draw their own borders for focused elements. These automatic borders can not be styled by Highcharts.
        /// In styled mode, the border is given the .highcharts-focus-border class.
        /// </summary>
        public FocusBorderStyle Style { get; set; }
    }
}
