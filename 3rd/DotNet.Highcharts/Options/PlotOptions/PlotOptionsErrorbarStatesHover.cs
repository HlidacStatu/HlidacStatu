using DotNet.Highcharts.Helpers;

namespace DotNet.Highcharts.Options.PlotOptions
{
	public class PlotOptionsErrorbarStatesHover
	{
		/// <summary>
		/// How much to brighten the point on interaction. Requires the main color to be defined in hex or rgb(a) format.
		/// Default: 0.1
		/// </summary>
		public Number? Brightness { get; set; }

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

		public PlotOptionsErrorbarStatesHoverMarker Marker { get; set; }

	}

}