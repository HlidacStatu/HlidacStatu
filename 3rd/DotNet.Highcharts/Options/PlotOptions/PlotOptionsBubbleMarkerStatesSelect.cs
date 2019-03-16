using System.Drawing;
using DotNet.Highcharts.Attributes;
using DotNet.Highcharts.Helpers;

namespace DotNet.Highcharts.Options.PlotOptions
{
	/// <summary>
	/// The appearance of the point marker when selected. In order to allow a point to be  selected, set the <code>series.allowPointSelect</code> option to true.
	/// </summary>
	public class PlotOptionsBubbleMarkerStatesSelect
	{
		/// <summary>
		/// Enable or disable visible feedback for selection.
		/// Default: true
		/// </summary>
		public bool? Enabled { get; set; }

		/// <summary>
		/// The fill color of the point marker. When <code>null</code>, the series' or point's color is used.
		/// </summary>
		[JsonFormatter(addPropertyName: true, useCurlyBracketsForObject: false)]
		public BackColorOrGradient FillColor { get; set; }

		/// <summary>
		/// The color of the point marker's outline. When <code>null</code>, the series' or point's color is used.
		/// Default: #000000
		/// </summary>
		public Color? LineColor { get; set; }

		/// <summary>
		/// The width of the point marker's outline.
		/// Default: 0
		/// </summary>
		public Number? LineWidth { get; set; }

		/// <summary>
		/// The radius of the point marker. In hover state, it defaults to the normal state's radius + 2.
		/// </summary>
		public Number? Radius { get; set; }

	}

}