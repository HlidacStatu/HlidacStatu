using System.Drawing;
using DotNet.Highcharts.Attributes;
using DotNet.Highcharts.Enums;
using DotNet.Highcharts.Helpers;

namespace DotNet.Highcharts.Options.PlotOptions
{
	/// <summary>
	/// Funnel charts are a type of chart often used to visualize stages in a sales project, where the top are the initial stages with the most clients. It requires that the <code>modules/funnel.js</code> file is loaded.
	/// </summary>
	public class PlotOptionsFunnel
	{
		/// <summary>
		/// Allow this series' points to be selected by clicking on the markers, bars or pie slices.
		/// Default: false
		/// </summary>
		public bool? AllowPointSelect { get; set; }

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
		/// The center of the funnel. By default, it is centered in the middle of the plot area, so it fills the plot area height.
		/// Default: ['50%', '50%']
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

		public PlotOptionsFunnelDataLabels DataLabels { get; set; }

		/// <summary>
		/// Enable or disable the mouse tracking for a specific series. This includes point tooltips and click events on graphs and points. For large datasets it improves performance.
		/// Default: true
		/// </summary>
		public bool? EnableMouseTracking { get; set; }

		public PlotOptionsFunnelEvents Events { get; set; }

		/// <summary>
		/// The height of the neck, the lower part of a funnel. If it is a number it defines the pixel height, if it is a percentage string it is the percentage of the plot area height.
		/// </summary>
		[JsonFormatter(addPropertyName: true, useCurlyBracketsForObject: false)]
		public PercentageOrPixel Height { get; set; }

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
		/// The <a href='#series.id'>id</a> of another series to link to. Additionally, the value can be ':previous' to link to the previous series. When two series are linked, only the first one appears in the legend. Toggling the visibility of this also toggles the linked series.
		/// </summary>
		public string LinkedTo { get; set; }

		/// <summary>
		/// The minimum size for a pie in response to auto margins. The pie will try to shrink to make room for data labels in side the plot area, but only to this size.
		/// Default: 80
		/// </summary>
		public Number? MinSize { get; set; }

		/// <summary>
		/// The width of the neck, the lower part of the funnel. A number defines pixel width, a percentage string defines a percentage of the plot area width.
		/// </summary>
		[JsonFormatter(addPropertyName: true, useCurlyBracketsForObject: false)]
		public PercentageOrPixel NeckWidth { get; set; }

		/// <summary>
		/// Properties for each single point
		/// </summary>
		public PlotOptionsFunnelPoint Point { get; set; }

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
		public PlotOptionsFunnelStates States { get; set; }

		/// <summary>
		/// Sticky tracking of mouse events. When true, the <code>mouseOut</code> event on a series isn't triggered until the mouse moves over another series, or out of the plot area. When false, the <code>mouseOut</code> event on a series is triggered when the mouse leaves the area around the series' graph or markers. This also implies the tooltip. When <code>stickyTracking</code> is false and <code>tooltip.shared</code> is false, the  tooltip will be hidden when moving the mouse between series.
		/// Default: false
		/// </summary>
		public bool? StickyTracking { get; set; }

		/// <summary>
		/// A configuration object for the tooltip rendering of each single series. Properties are inherited from <a href='#tooltip'>tooltip</a>, but only the following properties can be defined on a series level.
		/// </summary>
		public PlotOptionsFunnelTooltip Tooltip { get; set; }

		/// <summary>
		/// Set the initial visibility of the series.
		/// Default: true
		/// </summary>
		public bool? Visible { get; set; }

		/// <summary>
		/// The width of the funnel compared to the width of the plot area, or the pixel width if it is a number.
		/// Default: 90%
		/// </summary>
		[JsonFormatter(addPropertyName: true, useCurlyBracketsForObject: false)]
		public PercentageOrPixel Width { get; set; }

		/// <summary>
		/// Define the z index of the series.
		/// </summary>
		public Number? ZIndex { get; set; }

	}

}