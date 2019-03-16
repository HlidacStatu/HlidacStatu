using DotNet.Highcharts.Helpers;

namespace DotNet.Highcharts.Options.PlotOptions
{
	/// <summary>
	/// A configuration object for the tooltip rendering of each single series. Properties are inherited from <a href='#tooltip'>tooltip</a>, but only the following properties can be defined on a series level.
	/// </summary>
	public class PlotOptionsBarTooltip
	{
		/// <summary>
		/// <p>For series on a datetime axes, the date format in the tooltip's header will by default be guessed based on the closest data points. This member gives the default string representations used for each unit. For an overview of the replacement codes, see <a href='#Highcharts.dateFormat'>dateFormat</a>.</p><p>Defaults to:<pre>{ millisecond: '%H:%M:%S.%L', second: '%H:%M:%S', minute: '%H:%M', hour: '%H:%M', day: '%e. %b', week: '%e. %b', month: '%b \'%y', year: '%Y'}</pre></p>
		/// </summary>
		public DateTimeLabel DateTimeLabelFormats { get; set; }

		/// <summary>
		/// <p>Whether the tooltip should follow the mouse as it moves across columns, pie slices and other point types with an extent. By default it behaves this way for scatter, bubble and pie series by override in the <code>plotOptions</code> for those series types. </p><p>For touch moves to behave the same way, <a href='#tooltip.followTouchMove'>followTouchMove</a> must be <code>true</code> also.</p>
		/// Default: false
		/// </summary>
		public bool? FollowPointer { get; set; }

		/// <summary>
		/// Whether the tooltip should follow the finger as it moves on a touch device. The default value of <code>false</code> causes a touch move to scroll the web page, as is default behaviour on touch devices. Setting it to <code>true</code> may cause the user to be trapped inside the chart and unable to scroll away, so it should be used with care.
		/// Default: false
		/// </summary>
		public bool? FollowTouchMove { get; set; }

		/// <summary>
		/// A string to append to the tooltip format.
		/// Default: false
		/// </summary>
		public string FooterFormat { get; set; }

		/// <summary>
		/// <p>The HTML of the tooltip header line. Variables are enclosed by curly brackets. Available variablesare <code>point.key</code>, <code>series.name</code>, <code>series.color</code> and other members from the <code>point</code> and <code>series</code> objects. The <code>point.key</code> variable contains the category name, x value or datetime string depending on the type of axis. For datetime axes, the <code>point.key</code> date format can be set using tooltip.xDateFormat.</p> <p>Defaults to <code>&lt;span style='font-size: 10px'&gt;{point.key}&lt;/span&gt;&lt;br/&gt;</code></p>
		/// </summary>
		public string HeaderFormat { get; set; }

		/// <summary>
		/// The number of milliseconds to wait until the tooltip is hidden when mouse out from a point or chart. 
		/// Default: 500
		/// </summary>
		public Number? HideDelay { get; set; }

		/// <summary>
		/// <p>The HTML of the point's line in the tooltip. Variables are enclosed by curly brackets. Available variables are point.x, point.y, series.name and series.color and other properties on the same form. Furthermore,  point.y can be extended by the <code>tooltip.yPrefix</code> and <code>tooltip.ySuffix</code> variables. This can also be overridden for each series, which makes it a good hook for displaying units.</p> <p>Defaults to <code>&lt;span style='color:{series.color}'&gt;{series.name}&lt;/span&gt;: &lt;b&gt;{point.y}&lt;/b&gt;&lt;br/&gt;</code></p>
		/// </summary>
		public string PointFormat { get; set; }

		/// <summary>
		/// How many decimals to show in each series' y value. This is overridable in each series' tooltip options object. The default is to preserve all decimals.
		/// </summary>
		public Number? ValueDecimals { get; set; }

		/// <summary>
		/// A string to prepend to each series' y value. Overridable in each series' tooltip options object.
		/// </summary>
		public string ValuePrefix { get; set; }

		/// <summary>
		/// A string to append to each series' y value. Overridable in each series' tooltip options object.
		/// </summary>
		public string ValueSuffix { get; set; }

		/// <summary>
		/// The format for the date in the tooltip header if the X axis is a datetime axis. The default is a best guess based on the smallest distance between points in the chart.
		/// </summary>
		public string XDateFormat { get; set; }

	}

}