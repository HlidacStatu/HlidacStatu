namespace DotNet.Highcharts.Options.Accessibility
{
    /// <summary>
    /// Options for keyboard navigation.
    /// </summary>
    public class AccessibilityKeyboardNavigation
    {
        /// <summary>
        /// Enable keyboard navigation for the chart.
        /// Default: true
        /// </summary>
        public bool? Enabled { get; set; }
        /// <summary>
        /// Options for the focus border drawn around elements while navigating through them.
        /// </summary>
        public FocusBorder FocusBorder { get; set; }
        /// <summary>
        /// Set the keyboard navigation mode for the chart. Can be "normal" or "serialize". In normal mode, left/right arrow keys move between points in a series, while up/down arrow keys move between series. Up/down navigation acts intelligently to figure out which series makes sense to move to from any given point.
        /// In "serialize" mode, points are instead navigated as a single list. Left/right behaves as in "normal" mode. Up/down arrow keys will behave like left/right. This is useful for unifying navigation behavior with/without screen readers enabled.
        /// Default: normal
        /// </summary>
        public AccessibilityKeyboardNavigationModes? Mode { get; set; }
        /// <summary>
        /// Skip null points when navigating through points with the keyboard.
        /// Default: true
        /// </summary>
        public bool? SkipNullPoints { get; set; }
    }
}
