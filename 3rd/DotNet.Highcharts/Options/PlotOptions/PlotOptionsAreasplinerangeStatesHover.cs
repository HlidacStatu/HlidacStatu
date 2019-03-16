using DotNet.Highcharts.Helpers;

namespace DotNet.Highcharts.Options.PlotOptions
{
	public class PlotOptionsAreasplinerangeStatesHover
	{
		/// <summary>
		/// Enable separate styles for the hovered series to visualize that the user hovers either the series itself or the legend..
		/// Default: true
		/// </summary>
		public bool? Enabled { get; set; }

		/// <summary>
		/// Pixel with of the graph line.
		/// Default: 2
		/// </summary>
		public Number? LineWidth { get; set; }

		public PlotOptionsAreasplinerangeStatesHoverMarker Marker { get; set; }

	}

}