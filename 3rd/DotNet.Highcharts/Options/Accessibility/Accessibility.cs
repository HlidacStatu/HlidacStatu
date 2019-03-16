using DotNet.Highcharts.Attributes;

namespace DotNet.Highcharts.Options.Accessibility
{
    /// <summary>
    /// Options for configuring accessibility for the chart. Requires the accessibility module to be loaded. For a description of the module and information on its features, see Highcharts Accessibility.
    /// </summary>
    public class Accessibility
    {
        /// <summary>
        /// Whether or not to add series descriptions to charts with a single series.
        /// Default: false
        /// </summary>
        public bool? DescribeSingleSeries { get; set; }

        /// <summary>
        /// Enable accessibility features for the chart.
        /// Default: true
        /// </summary>
        public bool? Enabled { get; set; }

        /// <summary>
        /// Options for keyboard navigation.
        /// </summary>
        public AccessibilityKeyboardNavigation KeyboardNavigation { get; set; }

        /// <summary>
        /// Function to run upon clicking the "View as Data Table" link in the screen reader region.
        /// By default Highcharts will insert and set focus to a data table representation of the chart.
        /// Default: undefined
        /// </summary>
        [JsonFormatter("{0}")]
        public string OnTableAnchorClick { get; set; }

        /// <summary>
        /// Date format to use for points on datetime axes when describing them to screen reader users.
        /// Defaults to the same format as in tooltip.
        /// For an overview of the replacement codes, see dateFormat.
        /// Default: undefined
        /// </summary>
        public string PointDateFormat { get; set; }

        /// <summary>
        /// Formatter function to determine the date/time format used with points on datetime axes when describing them to screen reader users. Receives one argument, point, referring to the point to describe. Should return a date format string compatible with dateFormat.
        /// Default: undefined
        /// </summary>
        [JsonFormatter("{0}")]
        public string PointDateFormatter { get; set; }

        /// <summary>
        /// Formatter function to use instead of the default for point descriptions. Receives one argument, point, referring to the point to describe. Should return a String with the description of the point for a screen reader user.
        /// Default: undefined
        /// </summary>
        [JsonFormatter("{0}")]
        public string PointDescriptionFormatter { get; set; }

        /// <summary>
        /// pointDescriptionThreshold: Number, Boolean
        /// When a series contains more points than this, we no longer expose information about individual points to screen readers.
        /// Set to false to disable.
        /// Default: false
        /// </summary>
        public object PointDescriptionThreshold { get; set; }

        /// <summary>
        /// A formatter function to create the HTML contents of the hidden screen reader information region. Receives one argument, chart, referring to the chart object. Should return a String with the HTML content of the region.
        /// The link to view the chart as a data table will be added automatically after the custom HTML content.
        /// Default: undefined
        /// </summary>
        [JsonFormatter("{0}")]
        public string ScreenReaderSectionFormatter { get; set; }

        /// <summary>
        /// Formatter function to use instead of the default for series descriptions. Receives one argument, series, referring to the series to describe. Should return a String with the description of the series for a screen reader user.
        /// Default: undfined
        /// </summary>
        [JsonFormatter("{0}")]
        public string SeriesDescriptionFormatter { get; set; }
    }
}
