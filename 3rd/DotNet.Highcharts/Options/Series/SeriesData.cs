using System;
using System.Drawing;
using DotNet.Highcharts.Helpers;

namespace DotNet.Highcharts.Options.Series
{
	/// <summary>
	/// An array of data points for the series. The points can be given in three ways: <ol> <li>An array of numerical values. In this case, the numberical values will  be interpreted and y values, and x values will be automatically calculated, either starting at 0 and incrementing by 1, or from <code>pointStart</code>  and <code>pointInterval</code> given in the plotOptions. If the axis is has categories, these will be used. This option is not available for range series. Example:<pre>data: [0, 5, 3, 5]</pre> </li> <li><p>An array of arrays with two values. In this case, the first value is the x value and the second is the y value. If the first value is a string, it is applied as the name of the point, and the x value is incremented following the above rules.</p><p>For range series, the arrays will be interpreted as <code>[x, low, high]</code>. In this cases, the X value can be skipped altogether to make use of <code>pointStart</code> and <code>pointRange</code>.</p> Example:<pre>data: [[5, 2], [6, 3], [8, 2]]</pre></li><li><p>An array of objects with named values. In this case the objects are point configuration objects as seen below.</p><p>Range series values are given by <code>low</code> and <code>high</code>.</p>Example:<pre>data: [{ name: 'Point 1', color: '#00FF00', y: 0}, { name: 'Point 2', color: '#FF00FF', y: 5}]</pre></li> </ol>
	/// </summary>
	public class SeriesData
	{
		/// <summary>
		/// Individual color for the point. By default the color is pulled from the global <code>colors</code> array.
		/// </summary>
		public Color? Color { get; set; }

		/// <summary>
		/// Individual data label for each point. The options are the same as the ones for  <a class='internal' href='#plotOptions-series-dataLabels'>plotOptions.series.dataLabels</a>
		/// </summary>
		public Object DataLabels { get; set; }

		/// <summary>
		/// Individual point events
		/// </summary>
		public SeriesDataEvents Events { get; set; }

		/// <summary>
		/// An id for the point. This can be used after render time to get a pointer to the point object through <code>chart.get()</code>.
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// Pies only. The sequential index of the pie slice in the legend..
		/// </summary>
		public Number? LegendIndex { get; set; }

		public SeriesDataMarker Marker { get; set; }

		/// <summary>
		/// The name of the point as shown in the legend, tooltip, dataLabel etc.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Pie series only. Whether to display a slice offset from the center.
		/// Default: false
		/// </summary>
		public bool? Sliced { get; set; }

		/// <summary>
		/// The x value of the point.
		/// </summary>
		public Number? X { get; set; }

		/// <summary>
		/// The y value of the point.
		/// </summary>
		public Number? Y { get; set; }

	}

}