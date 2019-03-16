using System.Drawing;
using DotNet.Highcharts.Attributes;
using DotNet.Highcharts.Enums;
using DotNet.Highcharts.Helpers;

namespace DotNet.Highcharts.Options.PlotOptions
{
	/// <summary>
	/// General plotting options for the gauge series type. Requires <code>highcharts-more.js</code>
	/// </summary>
	public class PlotOptionsGauge
	{
		/// <summary>
		/// <p>Enable or disable the initial animation when a series is displayed. The animation can also be set as a configuration object. Please note that this option only applies to the initial animation of the series itself. For other animations, see <a href='#chart.animation'>chart.animation</a> and the animation parameter under the API methods.The following properties are supported:</p><dl>  <dt>duration</dt>  <dd>The duration of the animation in milliseconds.</dd><dt>easing</dt><dd>When using jQuery as the general framework, the easing can be set to <code>linear</code> or<code>swing</code>. More easing functions are available with the use of jQuery plug-ins, most notablythe jQuery UI suite. See <a href='http://api.jquery.com/animate/'>the jQuery docs</a>. When using MooTools as the general framework, use the property name <code>transition</code> instead of <code>easing</code>.</dd></dl><p>Due to poor performance, animation is disabled in old IE browsers for column charts and polar charts.</p>
		/// Default: true
		/// </summary>
		[JsonFormatter(addPropertyName: false, useCurlyBracketsForObject: false)]
		public Animation Animation { get; set; }

		/// <summary>
		/// The main color or the series. In line type series it applies to the line and the point markers unless otherwise specified. In bar type series it applies to the bars unless a color is specified per point. The default value is pulled from the  <code>options.colors</code> array.
		/// </summary>
		public Color? Color { get; set; }

		/// <summary>
		/// You can set the cursor to 'pointer' if you have click events attached to  the series, to signal to the user that the points and lines can be clicked.
		/// </summary>
		public Cursors? Cursor { get; set; }

		/// <summary>
		/// Data labels for the gauge. For gauges, the data labels are enabled by default and shown in a bordered box below the point.
		/// </summary>
		public PlotOptionsGaugeDataLabels DataLabels { get; set; }

		/// <summary>
		/// Options for the dial or arrow pointer of the gauge.
		/// </summary>
		public PlotOptionsGaugeDial Dial { get; set; }

		/// <summary>
		/// Enable or disable the mouse tracking for a specific series. This includes point tooltips and click events on graphs and points. For large datasets it improves performance.
		/// Default: true
		/// </summary>
		public bool? EnableMouseTracking { get; set; }

		public PlotOptionsGaugeEvents Events { get; set; }

		/// <summary>
		/// An id for the series. This can be used after render time to get a pointer to the series object through <code>chart.get()</code>.
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// The <a href='#series.id'>id</a> of another series to link to. Additionally, the value can be ':previous' to link to the previous series. When two series are linked, only the first one appears in the legend. Toggling the visibility of this also toggles the linked series.
		/// </summary>
		public string LinkedTo { get; set; }

		/// <summary>
		/// The color for the parts of the graph or points that are below the <a href='#plotOptions.series.threshold'>threshold</a>.
		/// Default: null
		/// </summary>
		public Color? NegativeColor { get; set; }

		/// <summary>
		/// Options for the pivot or the center point of the gauge.
		/// </summary>
		public PlotOptionsGaugePivot Pivot { get; set; }

		/// <summary>
		/// Properties for each single point
		/// </summary>
		public PlotOptionsGaugePoint Point { get; set; }

		/// <summary>
		/// Whether to select the series initially. If <code>showCheckbox</code> is true, the checkbox next to the series name will be checked for a selected series.
		/// Default: false
		/// </summary>
		public bool? Selected { get; set; }

		/// <summary>
		/// If true, a checkbox is displayed next to the legend item to allow selecting the series. The state of the checkbox is determined by the <code>selected</code> option.
		/// Default: false
		/// </summary>
		public bool? ShowCheckbox { get; set; }

		/// <summary>
		/// Whether to display this particular series or series type in the legend. Defaults to false for gauge series.
		/// </summary>
		public bool? ShowInLegend { get; set; }

		/// <summary>
		/// A wrapper object for all the series options in specific states.
		/// </summary>
		public PlotOptionsGaugeStates States { get; set; }

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
		public PlotOptionsGaugeTooltip Tooltip { get; set; }

		/// <summary>
		/// Set the initial visibility of the series.
		/// Default: true
		/// </summary>
		public bool? Visible { get; set; }

		/// <summary>
		/// When this option is <code>true</code>, the dial will wrap around the axes. For instance, in a full-range gauge going from 0 to 360, a value of 400 will point to 40. When <code>wrap</code> is <code>false</code>, the dial stops at 360.
		/// Default: true
		/// </summary>
		public bool? Wrap { get; set; }

		/// <summary>
		/// Define the z index of the series.
		/// </summary>
		public Number? ZIndex { get; set; }

	}

}