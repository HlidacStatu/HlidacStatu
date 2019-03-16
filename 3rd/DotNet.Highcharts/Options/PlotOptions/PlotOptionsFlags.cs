namespace DotNet.Highcharts.Options.PlotOptions
{
	public class PlotOptionsFlags : PlotOptionsSeries
	{
        /// <summary>
        /// The id of the series that the flags should be drawn on. 
        /// If no id is given, the flags are drawn on the x axis. 
        /// Defaults to undefined.
        /// </summary>
        public string OnSeries { get; set; }

        /// <summary>
        /// The shape of the marker. Can be one of "flag", "circlepin",
        /// "squarepin", or an image on the format url(/path-to-image.jpg).
        /// Individual shapes can also be set for each point. Defaults to flag.
        /// </summary>
        public string Shape { get; set; }
	}
}
