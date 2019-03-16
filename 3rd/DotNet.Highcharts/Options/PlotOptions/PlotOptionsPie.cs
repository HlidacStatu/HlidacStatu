using System.Drawing;
using DotNet.Highcharts.Attributes;
using DotNet.Highcharts.Enums;
using DotNet.Highcharts.Helpers;

namespace DotNet.Highcharts.Options.PlotOptions
{
	public class PlotOptionsPie
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
		/// The color of the border surronding each slice.
		/// Default: #FFFFFF
		/// </summary>
		public Color? BorderColor { get; set; }

		/// <summary>
		/// The width of the border surronding each slice.
		/// Default: 1
		/// </summary>
		public Number? BorderWidth { get; set; }

		/// <summary>
		/// The center of the pie chart relative to the plot area. Can be percentages or pixel values. The default behaviour (as of 3.0) is to center the pie so that all slices and data labels are within the plot area. As a consequence, the pie may actually jump around in a chart with dynamic values, as the data labels move. In that case, the center should be explicitly set, for example to <code>['50%', '50%']</code>. 
		/// Default: [null, null]
		/// </summary>
		public PercentageOrPixel[] Center { get; set; }

		/// <summary>
		/// A series specific or series type specific color set to use instead of the global <a href='#colors'>colors</a>.
		/// </summary>
		public Color[] Colors { get; set; }

		/// <summary>
		/// You can set the cursor to 'pointer' if you have click events attached to  the series, to signal to the user that the points and lines can be clicked.
		/// </summary>
		public Cursors? Cursor { get; set; }

		public PlotOptionsPieDataLabels DataLabels { get; set; }

        /// <summary>
        /// The thickness of a 3D pie. Requires highcharts-3d.js
        /// Default: 0
        /// </summary>
        public Number? Depth { get; set; }

		/// <summary>
		/// Enable or disable the mouse tracking for a specific series. This includes point tooltips and click events on graphs and points. For large datasets it improves performance.
		/// Default: true
		/// </summary>
		public bool? EnableMouseTracking { get; set; }

		public PlotOptionsPieEvents Events { get; set; }

		/// <summary>
		/// An id for the series. This can be used after render time to get a pointer to the series object through <code>chart.get()</code>.
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// <p>Equivalent to <a href='#chart.ignoreHiddenSeries'>chart.ignoreHiddenSeries</a>, this option tells whether the series shall be redrawn as if the hidden point were <code>null</code>.</p><p>The default value changed from <code>false</code> to <code>true</code> with Highcharts 3.0.</p>
		/// Default: true
		/// </summary>
		public bool? IgnoreHiddenPoint { get; set; }

		/// <summary>
		/// The size of the inner diameter for the pie. A size greater than 0 renders a donut chart. Can be a percentage or pixel value. Percentages are relative to the size of the plot area. Pixel values are given as integers.
		/// Default: 0
		/// </summary>
		[JsonFormatter(addPropertyName: true, useCurlyBracketsForObject: false)]
		public PercentageOrPixel InnerSize { get; set; }

		/// <summary>
		/// The <a href='#series.id'>id</a> of another series to link to. Additionally, the value can be ':previous' to link to the previous series. When two series are linked, only the first one appears in the legend. Toggling the visibility of this also toggles the linked series.
		/// </summary>
		public string LinkedTo { get; set; }

		/// <summary>
		/// The minimum size for a pie in response to auto margins. The pie will try to shrink to make room for data labels in side the plot area, but only to this size.
		/// Default: 80
		/// </summary>
		public Number? MinSize { get; set; }

		/// <summary>
		/// Properties for each single point
		/// </summary>
		public PlotOptionsPiePoint Point { get; set; }

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
		/// Whether to display this particular series or series type in the legend. Since 2.1, pies are not shown in the legend by default.
		/// Default: false
		/// </summary>
		public bool? ShowInLegend { get; set; }

		/// <summary>
		/// The diameter of the pie relative to the plot area. Can be a percentage or pixel value. Pixel values are given as integers. The default behaviour (as of 3.0) is to scale to the plot area and give room for data labels within the plot area. As a consequence, the size of the pie may vary when points are updated and data labels more around. In that case it is best to set a fixed value, for example <code>'75%'</code>.
		/// Default:  
		/// </summary>
		[JsonFormatter(addPropertyName: true, useCurlyBracketsForObject: false)]
		public PercentageOrPixel Size { get; set; }

		/// <summary>
		/// If a point is sliced, moved out from the center, how many pixels should  it be moved?.
		/// Default: 10
		/// </summary>
		public Number? SlicedOffset { get; set; }

		/// <summary>
		/// The start angle of the pie slices in degrees where 0 is top and 90 right. 
		/// Default: 0
		/// </summary>
		public Number? StartAngle { get; set; }

		/// <summary>
		/// A wrapper object for all the series options in specific states.
		/// </summary>
		public PlotOptionsPieStates States { get; set; }

		/// <summary>
		/// Sticky tracking of mouse events. When true, the <code>mouseOut</code> event on a series isn't triggered until the mouse moves over another series, or out of the plot area. When false, the <code>mouseOut</code> event on a series is triggered when the mouse leaves the area around the series' graph or markers. This also implies the tooltip. When <code>stickyTracking</code> is false and <code>tooltip.shared</code> is false, the  tooltip will be hidden when moving the mouse between series.
		/// Default: false
		/// </summary>
		public bool? StickyTracking { get; set; }

		/// <summary>
		/// A configuration object for the tooltip rendering of each single series. Properties are inherited from <a href='#tooltip'>tooltip</a>, but only the following properties can be defined on a series level.
		/// </summary>
		public PlotOptionsPieTooltip Tooltip { get; set; }

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