using System.Drawing;
using DotNet.Highcharts.Attributes;
using DotNet.Highcharts.Helpers;

namespace DotNet.Highcharts.Options.PlotOptions
{
	public class PlotOptionsSplineStatesHoverMarker
	{
		/// <summary>
		/// Enable or disable the point marker.
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
		/// Default: #FFFFFF
		/// </summary>
		public Color? LineColor { get; set; }

		/// <summary>
		/// The width of the point marker's outline.
		/// Default: 0
		/// </summary>
		public Number? LineWidth { get; set; }

		/// <summary>
		/// The radius of the point marker.
		/// Default: 0
		/// </summary>
		public Number? Radius { get; set; }

		/// <summary>
		/// A predefined shape or symbol for the marker. When null, the symbol is pulled from options.symbols. Other possible values are 'circle', 'square', 'diamond', 'triangle' and 'triangle-down'. Additionally, the URL to a graphic can be given on this form:  'url(graphic.png)'.
		/// </summary>
		public string Symbol { get; set; }

	}

}