namespace DotNet.Highcharts.Options.PlotOptions
{
	/// <summary>
	/// <p>The plotOptions is a wrapper object for config objects for each series type. The config objects for each series can also be overridden for each series  item as given in the series array.</p> <p>Configuration options for the series are given in three levels. Options for all series in a chart are given in the <a class='internal' href='#plotOptions.series'>plotOptions.series</a> object. Then options for all series of a specific type are given in the plotOptions of that type, for example plotOptions.line. Next, options for one single series are given in <a class='internal' href='#series'>the  series array</a>.</p>
	/// </summary>
	public class PlotOptions
	{
		public PlotOptionsArea Area { get; set; }

		/// <summary>
		/// The area range is a cartesian series type with higher and lower Y values along an X axis. Requires <code>highcharts-more.js</code>.
		/// </summary>
		public PlotOptionsArearange Arearange { get; set; }

        /// <summary>
        /// Areaspline plot options.
        /// </summary>
		public PlotOptionsAreaspline Areaspline { get; set; }

		/// <summary>
		/// The area spline range is a cartesian series type with higher and lower Y values along an X axis. Requires <code>highcharts-more.js</code>.
		/// </summary>
		public PlotOptionsAreasplinerange Areasplinerange { get; set; }

        /// <summary>
        /// Bar chart options.
        /// </summary>
		public PlotOptionsBar Bar { get; set; }

		/// <summary>
		/// A box plot is a convenient way of depicting groups of data through their five-number summaries: the smallest observation (sample minimum), lower quartile (Q1), median (Q2), upper quartile (Q3), and largest observation (sample maximum). 
		/// </summary>
		public PlotOptionsBoxplot Boxplot { get; set; }

		/// <summary>
		/// A bubble series is a three dimensional series type where each point renders an X, Y and Z value. Each points is drawn as a bubble where the position along the X and Y axes mark the X and Y values, and the size of the bubble relates to the Z value.
		/// </summary>
		public PlotOptionsBubble Bubble { get; set; }

        /// <summary>
        /// Column chart options.
        /// </summary>
		public PlotOptionsColumn Column { get; set; }

		/// <summary>
		/// The column range is a cartesian series type with higher and lower Y values along an X axis. Requires <code>highcharts-more.js</code>. To display horizontal bars, set <a href='#chart.inverted'>chart.inverted</a> to <code>true</code>.
		/// </summary>
		public PlotOptionsColumnrange Columnrange { get; set; }

		/// <summary>
		/// Error bars are a graphical representation of the variability of data and are used on graphs to indicate the error, or uncertainty in a reported measurement. 
		/// </summary>
		public PlotOptionsErrorbar Errorbar { get; set; }

		/// <summary>
		/// Funnel charts are a type of chart often used to visualize stages in a sales project, where the top are the initial stages with the most clients. It requires that the <code>modules/funnel.js</code> file is loaded.
		/// </summary>
		public PlotOptionsFunnel Funnel { get; set; }

		/// <summary>
		/// General plotting options for the gauge series type. Requires <code>highcharts-more.js</code>
		/// </summary>
		public PlotOptionsGauge Gauge { get; set; }

        /// <summary>
        /// Line chart options.
        /// </summary>
		public PlotOptionsLine Line { get; set; }

        /// <summary>
        /// Pie chart options.
        /// </summary>
		public PlotOptionsPie Pie { get; set; }

        /// <summary>
        /// Scatter chart options.
        /// </summary>
		public PlotOptionsScatter Scatter { get; set; }

		/// <summary>
		/// <p>General options for all series types.</p>
		/// </summary>
		public PlotOptionsSeries Series { get; set; }

        /// <summary>
        /// Spline chart options.
        /// </summary>
		public PlotOptionsSpline Spline { get; set; }

		/// <summary>
		/// Options for the waterfall series type.
		/// </summary>
		public PlotOptionsWaterfall Waterfall { get; set; }

        /// <summary>
        /// OHLC chart options.
        /// </summary>
        public PlotOptionsOhlc Ohlc { get; set; }

        /// <summary>
        /// Candlestick chart options.
        /// </summary>
        public PlotOptionsCandlestick Candlestick { get; set; }

	}

}