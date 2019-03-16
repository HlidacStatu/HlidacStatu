using System.Drawing;
using DotNet.Highcharts.Helpers;

namespace DotNet.Highcharts.Options.XAxis
{
	/// <summary>
	/// <p>A colored band stretching across the plot area marking an interval on the axis.</p><p>In a gauge, a plot band on the Y axis (value axis) will stretch along the perimiter of the gauge.</p>
	/// </summary>
	public class XAxisPlotBands
	{
        /// <summary>
        /// Border color for the plot band. Also requires borderWidth to be set.
        /// Default: undefined
        /// </summary>
        public Color? BorderColor { get; set; }

        /// <summary>
        /// Border width for the plot band. Also requires borderColor to be set.
        /// Default: 0
        /// </summary>
        public Number? BorderWidth { get; set; }

        /// <summary>
        /// A custom class name, in addition to the default highcharts-plot-band, to apply to each individual band.
        /// Default: undefined
        /// </summary>
        public string ClassName { get; set; }

		/// <summary>
		/// The color of the plot band.
		/// </summary>
		public Color? Color { get; set; }

		/// <summary>
		/// An object defining mouse events for the plot band. Supported properties are <code>click</code>, <code>mouseover</code>, <code>mouseout</code>, <code>mousemove</code>.
		/// </summary>
		public Events Events { get; set; }

		/// <summary>
		/// The start position of the plot band in axis units.
		/// </summary>
		public Number? From { get; set; }

		/// <summary>
		/// An id used for identifying the plot band in Axis.removePlotBand.
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// Text labels for the plot bands
		/// </summary>
		public XAxisPlotBandsLabel Label { get; set; }

		/// <summary>
		/// The end position of the plot band in axis units.
		/// </summary>
		public Number? To { get; set; }

		/// <summary>
		/// The z index of the plot band within the chart.
		/// </summary>
		public Number? ZIndex { get; set; }

	}

}