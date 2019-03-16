using DotNet.Highcharts.Attributes;

namespace DotNet.Highcharts.Options.PlotOptions
{
	public class PlotOptionsAreasplinerangeEvents
	{
		/// <summary>
		/// Fires when the checkbox next to the series' name in the legend is clicked.. The <code>this</code> keyword refers to the  series object itself. One parameter, <code>event</code>, is passed to the function. The state of the checkbox is found by <code>event.checked</code>. Return <code>false</code> to prevent the default action which is to toggle the select state of the series.
		/// </summary>
		[JsonFormatter("{0}")]
		public string CheckboxClick { get; set; }

		/// <summary>
		/// Fires when the series is clicked. The <code>this</code> keyword refers to the  series object itself. One parameter, <code>event</code>, is passed to the function. This contains common event information based on jQuery or MooTools depending on  which library is used as the base for Highcharts. Additionally, <code>event.point</code> holds a pointer to the nearest point on the graph.
		/// </summary>
		[JsonFormatter("{0}")]
		public string Click { get; set; }

		/// <summary>
		/// Fires when the series is hidden after chart generation time, either by clicking the legend item or by calling <code>.hide()</code>.
		/// </summary>
		[JsonFormatter("{0}")]
		public string Hide { get; set; }

		/// <summary>
		/// Fires when the legend item belonging to the series is clicked.  The <code>this</code> keyword refers to the  series object itself. One parameter, <code>event</code>, is passed to the function. This contains common event information based on jQuery or MooTools depending on  which library is used as the base for Highcharts. The default action is to toggle the visibility of the series. This can be prevented by returning <code>false</code> or calling <code>event.preventDefault()</code>.
		/// </summary>
		[JsonFormatter("{0}")]
		public string LegendItemClick { get; set; }

		/// <summary>
		/// Fires when the mouse leaves the graph. The <code>this</code> keyword refers to the  series object itself. One parameter, <code>event</code>, is passed to the function. This contains common event information based on jQuery or MooTools depending on  which library is used as the base for Highcharts. If the  <a class='internal' href='#plotOptions-series'>stickyTracking</a> option is true, <code>mouseOut</code> doesn't happen before the mouse enters another graph or leaves the plot area.
		/// </summary>
		[JsonFormatter("{0}")]
		public string MouseOut { get; set; }

		/// <summary>
		/// Fires when the mouse enters the graph. The <code>this</code> keyword refers to the  series object itself. One parameter, <code>event</code>, is passed to the function. This contains common event information based on jQuery or MooTools depending on  which library is used as the base for Highcharts.
		/// </summary>
		[JsonFormatter("{0}")]
		public string MouseOver { get; set; }

		/// <summary>
		/// Fires when the series is shown after chart generation time, either by clicking the legend item or by calling <code>.show()</code>.
		/// </summary>
		[JsonFormatter("{0}")]
		public string Show { get; set; }

	}

}