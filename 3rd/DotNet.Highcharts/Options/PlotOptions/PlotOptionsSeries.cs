using System.Drawing;
using DotNet.Highcharts.Attributes;
using DotNet.Highcharts.Enums;
using DotNet.Highcharts.Helpers;

namespace DotNet.Highcharts.Options.PlotOptions
{
    /// <summary>
    /// <p>General options for all series types.</p>
    /// </summary>
    public class PlotOptionsSeries
	{
		/// <summary>
		/// Allow this series' points to be selected by clicking on the markers, bars or pie slices.
		/// Default: false
		/// </summary>
		public bool? AllowPointSelect { get; set; }

		/// <summary>
		/// <p>Enable or disable the initial animation when a series is displayed. The animation can also be set as a configuration object. Please note that this option only applies to the initial animation of the series itself. For other animations, see <a href='#chart.animation'>chart.animation</a> and the animation parameter under the API methods.The following properties are supported:</p><dl>  <dt>duration</dt>  <dd>The duration of the animation in milliseconds.</dd><dt>easing</dt><dd>When using jQuery as the general framework, the easing can be set to <code>linear</code> or<code>swing</code>. More easing functions are available with the use of jQuery plug-ins, most notablythe jQuery UI suite. See <a href='http://api.jquery.com/animate/'>the jQuery docs</a>. When using MooTools as the general framework, use the property name <code>transition</code> instead of <code>easing</code>.</dd></dl><p>Due to poor performance, animation is disabled in old IE browsers for column charts and polar charts.</p>
		/// Default: true
		/// </summary>
		[JsonFormatter(addPropertyName: false, useCurlyBracketsForObject: false)]
		public Animation Animation { get; set; }

        /// <summary>
        /// For some series, there is a limit that shuts down initial animation by default when the total number of points in the chart is too high. For example, for a column chart and its derivatives, animation doesn't run if there is more than 250 points totally. To disable this cap, set animationLimit to Infinity.
        /// Default: undefined
        /// </summary>
        public Number? AnimationLimit { get; set; }

        /// <summary>
        /// Set the point threshold for when a series should enter boost mode.
        /// Setting it to e.g. 2000 will cause the series to enter boost mode when there are 2000 or more points in the series.
        /// To disable boosting on the series, set the boostThreshold to 0. Setting it to 1 will force boosting.
        /// Requires modules/boost.js.
        /// Default: 5000
        /// </summary>
        public Number? BoostThreshold { get; set; }

        /// <summary>
        /// The border color of the map areas.
        /// In styled mode, the border stroke is given in the .highcharts-point class.
        /// Default: #cccccc
        /// </summary>
        public Color? BorderColor { get; set; }

        /// <summary>
        /// The border width of each map area.
        /// In styled mode, the border stroke width is given in the .highcharts-point class.
        /// Default: undefined
        /// </summary>
        public Number? BorderWidth { get; set; }

        /// <summary>
        /// An additional class name to apply to the series' graphical elements. This option does not replace default class names of the graphical element.
        /// Default: undefined
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// Disable this option to allow series rendering in the whole plotting area.
        /// Note: Clipping should be always enabled when chart.zoomType is set
        /// Default: false
        /// </summary>
        public bool? Clip { get; set; }

		/// <summary>
		/// The main color or the series. In line type series it applies to the line and the point markers unless otherwise specified. In bar type series it applies to the bars unless a color is specified per point. The default value is pulled from the  <code>options.colors</code> array.
		/// </summary>
		public Color? Color { get; set; }

        /// <summary>
        /// Styled mode only. A specific color index to use for the series, so its graphic representations are given the class name highcharts-color-{n}.
        /// Default: undefined
        /// </summary>
        public Number? ColorIndex { get; set; }

		/// <summary>
		/// Polar charts only. Whether to connect the ends of a line series plot across the extremes.
		/// Default: true
		/// </summary>
		public bool? ConnectEnds { get; set; }

		/// <summary>
		/// Whether to connect a graph line across null points.
		/// Default: false
		/// </summary>
		public bool? ConnectNulls { get; set; }

		/// <summary>
		/// When the series contains less points than the crop threshold, all points are drawn,  event if the points fall outside the visible plot area at the current zoom. The advantage of drawing all points (including markers and columns), is that animation is performed on updates. On the other hand, when the series contains more points than the crop threshold, the series data is cropped to only contain points that fall within the plot area. The advantage of cropping away invisible points is to increase performance on large series.  .
		/// Default: 300
		/// </summary>
		public Number? CropThreshold { get; set; }

		/// <summary>
		/// You can set the cursor to 'pointer' if you have click events attached to  the series, to signal to the user that the points and lines can be clicked.
		/// </summary>
		public Cursors? Cursor { get; set; }

		/// <summary>
		/// A name for the dash style to use for the graph. Applies only to series type having a graph, like <code>line</code>, <code>spline</code>, <code>area</code> and <code>scatter</code> in  case it has a <code>lineWidth</code>. The value for the <code>dashStyle</code> include:    <ul>    <li>Solid</li>    <li>ShortDash</li>    <li>ShortDot</li>    <li>ShortDashDot</li>    <li>ShortDashDotDot</li>    <li>Dot</li>    <li>Dash</li>    <li>LongDash</li>    <li>DashDot</li>    <li>LongDashDot</li>    <li>LongDashDotDot</li>    </ul>.
		/// </summary>
		public DashStyles? DashStyle { get; set; }

        /// <summary>
        /// Options regarding the grouping of data for the plotting.
        /// </summary>
        public DataGrouping DataGrouping { get; set; }

        /// <summary>
        /// Defines the appearance of the data labels, static labels for each point.
        /// </summary>
		public PlotOptionsSeriesDataLabels DataLabels { get; set; }

        /// <summary>
        /// Requires the Accessibility module
        /// A description of the series to add to the screen reader information about the series.
        /// Default: undefined
        /// </summary>
        public string Description { get; set; }

		/// <summary>
		/// Enable or disable the mouse tracking for a specific series. This includes point tooltips and click events on graphs and points. For large datasets it improves performance.
		/// Default: true
		/// </summary>
		public bool? EnableMouseTracking { get; set; }

        /// <summary>
        /// General event handlers for the series items. These event hooks can also be attached to the series at run time using the Highcharts.addEvent function.
        /// </summary>
		public PlotOptionsSeriesEvents Events { get; set; }

        /// <summary>
        /// By default, series are exposed to screen readers as regions. By enabling this option, the series element itself will be exposed in the same way as the data points. This is useful if the series is not used as a grouping entity in the chart, but you still want to attach a description to the series.
        /// Requires the Accessibility module
        /// Default: undefined
        /// </summary>
        public bool? ExposeElementToAlly { get; set; }

        /// <summary>
        /// Determines whether the series should look for the nearest point in both dimensions or just the x-dimension when hovering the series. Defaults to 'xy' for scatter series and 'x' for most other series. If the data has duplicate x-values, it is recommended to set this to 'xy' to allow hovering over all points.
        /// Applies only to series types using nearest neighbor search (not direct hover) for tooltip.
        /// Default: x
        /// </summary>
        public string FindNearestPointBy { get; set; }

        /// <summary>
        /// Defines when to display a gap in the graph. A gap size of 5 means that if the distance
        /// between two points is greater than five times that of the two closest
        /// points, the graph will be broken.
        /// In practice, this option is most often used to visualize gaps in time series.
        /// In a stock chart, intraday data is available for daytime hours,
        /// while gaps will appear in nights and weekends.
        /// Defaults to 0.
        /// </summary>
        public Number? GapSize { get; set; }

        /// <summary>
        /// Whether to use the Y extremes of the total chart width or only the zoomed area when zooming in on parts of the X axis. By default, the Y axis adjusts to the min and max of the visible data. Cartesian series only.
        /// Default: false
        /// </summary>
        public bool? GetExtremesFromAll { get; set; }

		/// <summary>
		/// An id for the series. This can be used after render time to get a pointer to the series object through <code>chart.get()</code>.
		/// </summary>
		public string Id { get; set; }

        /// <summary>
        /// An array specifying which option maps to which key in the data point array. This makes it convenient to work with unstructured data arrays from different sources.
        /// Default: undefined
        /// </summary>
        public string[] Keys { get; set; }

		/// <summary>
		/// Pixel with of the graph line.
		/// Default: 2
		/// </summary>
		public Number? LineWidth { get; set; }

		/// <summary>
		/// The <a href='#series.id'>id</a> of another series to link to. Additionally, the value can be ':previous' to link to the previous series. When two series are linked, only the first one appears in the legend. Toggling the visibility of this also toggles the linked series.
		/// </summary>
		public string LinkedTo { get; set; }

		public PlotOptionsSeriesMarker Marker { get; set; }

		/// <summary>
		/// The color for the parts of the graph or points that are below the <a href='#plotOptions.series.threshold'>threshold</a>.
		/// Default: null
		/// </summary>
		public Color? NegativeColor { get; set; }

		/// <summary>
		/// Properties for each single point
		/// </summary>
		public PlotOptionsSeriesPoint Point { get; set; }

		/// <summary>
		/// <p>If no x values are given for the points in a series, pointInterval defines the interval of the x values. For example, if a series contains one value every decade starting from year 0, set pointInterval to 10.</p>.
		/// Default: 1
		/// </summary>
		public Number? PointInterval { get; set; }

	    /// <summary>
	    /// <p>Possible values: null, 'on', 'between', number.</p>
	    /// <p>In a column chart, when pointPlacement is 'on', the point will not create any padding of the X axis. In a polar column chart this means that the first column points directly north. If the pointPlacement is 'between', the columns will be laid out between ticks. This is useful for example for visualising an amount between two points in time or in a certain sector of a polar chart.</p>
	    /// <p>Defaults to <code>null</code> in cartesian charts, <code>'between'</code> in polar charts</p>.
	    /// Since Highcharts 3.0.2, the point placement can also be numeric, where 0 is on the axis value, -0.5 is between this value and the previous, and 0.5 is between this value and the next. Unlike the textual options, numeric point placement options won't affect axis padding.
	    /// Note that pointPlacement needs a pointRange to work. For column series this is computed, but for line-type series it needs to be set.
	    /// Defaults to undefined in cartesian charts, "between" in polar charts.
	    /// Default: undefined
	    /// </summary>
        public object PointPlacement { get; set; }

		/// <summary>
		/// If no x values are given for the points in a series, pointStart defines on what value to start. For example, if a series contains one yearly value starting from 1945, set pointStart to 1945.
		/// Default: 0
		/// </summary>
		[JsonFormatter(addPropertyName: false, useCurlyBracketsForObject: false)]
		public PointStart PointStart { get; set; }

		/// <summary>
		/// Whether to select the series initially. If <code>showCheckbox</code> is true, the checkbox next to the series name will be checked for a selected series.
		/// Default: false
		/// </summary>
		public bool? Selected { get; set; }

		/// <summary>
		/// Whether to apply a drop shadow to the graph line. Since 2.3 the shadow can be an object configuration containing <code>color</code>, <code>offsetX</code>, <code>offsetY</code>, <code>opacity</code> and <code>width</code>.
		/// Default: false
		/// </summary>
		public bool? Shadow { get; set; }

		/// <summary>
		/// If true, a checkbox is displayed next to the legend item to allow selecting the series. The state of the checkbox is determined by the <code>selected</code> option.
		/// Default: false
		/// </summary>
		public bool? ShowCheckbox { get; set; }

		/// <summary>
		/// Whether to display this particular series or series type in the legend.
		/// Default: true
		/// </summary>
		public bool? ShowInLegend { get; set; }

		/// <summary>
		/// Whether to stack the values of each series on top of each other. Possible values are null to disable, 'normal' to stack by value or 'percent'.
		/// </summary>
		public Stackings? Stacking { get; set; }

		/// <summary>
		/// A wrapper object for all the series options in specific states.
		/// </summary>
		public PlotOptionsSeriesStates States { get; set; }

		/// <summary>
		/// Sticky tracking of mouse events. When true, the <code>mouseOut</code> event on a series isn't triggered until the mouse moves over another series, or out of the plot area. When false, the <code>mouseOut</code> event on a series is triggered when the mouse leaves the area around the series' graph or markers. This also implies the tooltip. When <code>stickyTracking</code> is false and <code>tooltip.shared</code> is false, the  tooltip will be hidden when moving the mouse between series.
		/// Default: true
		/// </summary>
		public bool? StickyTracking { get; set; }

		/// <summary>
		/// The threshold, also called zero level or base level. For line type series this is only used in conjunction with <a href='#plotOptions.series.negativeColor'>negativeColor</a>.
		/// Default: 0
		/// </summary>
		public Number? Threshold { get; set; }

		/// <summary>
		/// A configuration object for the tooltip rendering of each single series. Properties are inherited from <a href='#tooltip'>tooltip</a>, but only the following properties can be defined on a series level.
		/// </summary>
		public PlotOptionsSeriesTooltip Tooltip { get; set; }

		/// <summary>
		/// When a series contains a data array that is longer than this, only one dimensional arrays of numbers, or two dimensional arrays with x and y values are allowed. Also, only the first point is tested, and the rest are assumed to be the same format. This saves expensive data checking and indexing in long series.
		/// Default: 1000
		/// </summary>
		public Number? TurboThreshold { get; set; }

		/// <summary>
		/// Set the initial visibility of the series.
		/// Default: true
		/// </summary>
		public bool? Visible { get; set; }

		/// <summary>
		/// Define the z index of the series.
		/// </summary>
		public Number? ZIndex { get; set; }

	}

}