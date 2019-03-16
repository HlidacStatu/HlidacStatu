using System.Drawing;
using DotNet.Highcharts.Enums;
using DotNet.Highcharts.Helpers;

namespace DotNet.Highcharts.Options.XAxis
{
	/// <summary>
	/// A line streching across the plot area, marking a specific value on one of the axes.
	/// </summary>
	public class XAxisPlotLines
	{
		/// <summary>
		/// The color of the line.
		/// </summary>
		public Color? Color { get; set; }

		/// <summary>
		/// The dashing or dot style for the plot line. For possible values see <a href='http://jsfiddle.net/gh/get/jquery/1.7.1/highslide-software/highcharts.com/tree/master/samples/highcharts/plotoptions/series-dashstyle-all/'> this overview</a>.
		/// Default: Solid
		/// </summary>
		public DashStyles? DashStyle { get; set; }

		/// <summary>
		/// An object defining mouse events for the plot line. Supported properties are <code>click</code>, <code>mouseover</code>, <code>mouseout</code>, <code>mousemove</code>.
		/// </summary>
		public Events Events { get; set; }

		/// <summary>
		/// An id used for identifying the plot line in Axis.removePlotLine.
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// Text labels for the plot bands
		/// </summary>
		public XAxisPlotLinesLabel Label { get; set; }

		/// <summary>
		/// The position of the line in axis units.
		/// </summary>
		public Number? Value { get; set; }

		/// <summary>
		/// The width or thickness of the plot line.
		/// </summary>
		public Number? Width { get; set; }

		/// <summary>
		/// The z index of the plot line within the chart.
		/// </summary>
		public Number? ZIndex { get; set; }

	}

}