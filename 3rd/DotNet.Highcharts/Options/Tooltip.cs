using System;
using System.Drawing;
using DotNet.Highcharts.Enums;
using DotNet.Highcharts.Attributes;
using DotNet.Highcharts.Helpers;

namespace DotNet.Highcharts.Options
{
	/// <summary>
	/// Options for the tooltip that appears when the user hovers over a series or point.
	/// </summary>
	public class Tooltip
	{
		/// <summary>
		/// Enable or disable animation of the tooltip. In slow legacy IE browsers the animation is disabled by default.
		/// Default: true
		/// </summary>
		[JsonFormatter(addPropertyName: false, useCurlyBracketsForObject: false)]
		public Animation Animation { get; set; }

		/// <summary>
		/// The background color or gradient for the tooltip.
		/// Default: rgba(255, 255, 255, 0.85)
		/// </summary>
		[JsonFormatter(addPropertyName: true, useCurlyBracketsForObject: false)]
		public BackColorOrGradient BackgroundColor { get; set; }

		/// <summary>
		/// The color of the tooltip border. When <code>null</code>, the border takes the color of the corresponding series or point.
		/// Default: auto
		/// </summary>
		public Color? BorderColor { get; set; }

		/// <summary>
		/// The radius of the rounded border corners.
		/// Default: 3
		/// </summary>
		public Number? BorderRadius { get; set; }

		/// <summary>
		/// The pixel width of the tooltip border.
		/// Default: 1
		/// </summary>
		public Number? BorderWidth { get; set; }

		/// <summary>
		/// Display crosshairs to connect the points with their corresponding axis values. The crosshairs can be defined as a boolean, an array of booleans or an object. <dl> <dt>Boolean</dt> <dd>If the crosshairs option is true, a single crosshair relating to the x axis will be shown.</dd>  <dt>Array of booleans</dt> <dd>In an array of booleans, the first value turns on the x axis crosshair and the second value to the y axis crosshair. Use <code>[true, true]</code> to show complete crosshairs.</dd>  <dt>Array of objects</dt> <dd>In an array of objects, the first value applies to the x axis crosshair and the second value to the y axis crosshair. For each dimension, a <code>width</code>, <code>color</code>, <code><a href='http://jsfiddle.net/gh/get/jquery/1.7.1/highslide-software/highcharts.com/tree/master/samples/highcharts/plotoptions/series-dashstyle-all/'>dashStyle</a></code> and <code>zIndex</code> can be given.</dd> </dl> Defaults to <code>null</code>.
		/// </summary>
		[JsonFormatter(addPropertyName: false, useCurlyBracketsForObject: false)]
		public Crosshairs Crosshairs { get; set; }

		/// <summary>
		/// <p>For series on a datetime axes, the date format in the tooltip's header will by default be guessed based on the closest data points. This member gives the default string representations used for each unit. For an overview of the replacement codes, see <a href='#Highcharts.dateFormat'>dateFormat</a>.</p><p>Defaults to:<pre>{ millisecond: '%H:%M:%S.%L', second: '%H:%M:%S', minute: '%H:%M', hour: '%H:%M', day: '%e. %b', week: '%e. %b', month: '%b \'%y', year: '%Y'}</pre></p>
		/// </summary>
		public DateTimeLabel DateTimeLabelFormats { get; set; }

		/// <summary>
		/// Enable or disable the tooltip.
		/// Default: true
		/// </summary>
		public bool? Enabled { get; set; }

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
		/// <p>Callback function to format the text of the tooltip. Return false to disable tooltip for a specific point on series.</p> <p>A subset of HTML is supported. The HTML of the tooltip is parsed and converted to SVG,  therefore this isn't a complete HTML renderer. The following tabs are supported:  <code>&lt;b&gt;</code>, <code>&lt;strong&gt;</code>, <code>&lt;i&gt;</code>, <code>&lt;em&gt;</code>, <code>&lt;br/&gt;</code>, <code>&lt;span&gt;</code>. Spans can be styled with a <code>style</code> attribute, but only text-related CSS that is  shared with SVG is handled. </p> <p>Since version 2.1 the tooltip can be shared between multiple series through  the <code>shared</code> option. The available data in the formatter differ a bit depending on whether the tooltip is shared or not. In a shared tooltip, all  properties except <code>x</code>, which is common for all points, are kept in  an array, <code>this.points</code>.</p>  <p>Available data are:</p> <dl> <dt>this.percentage (not shared) / this.points[i].percentage (shared)</dt> <dd>Stacked series and pies only. The point's percentage of the total.</dd>  <dt>this.point (not shared) / this.points[i].point (shared)</dt> <dd>The point object. The point name, if defined, is available  through <code>this.point.name</code>.</dd>  <dt>this.points</dt> <dd>In a shared tooltip, this is an array containing all other properties for each point.</dd>  <dt>this.series (not shared) / this.points[i].series (shared)</dt> <dd>The series object. The series name is available  through <code>this.series.name</code>.</dd>  <dt>this.total (not shared) / this.points[i].total (shared)</dt> <dd>Stacked series only. The total value at this point's x value.</dd>  <dt>this.x</dt> <dd>The x value. This property is the same regardless of the tooltip being shared or not.</dd>  <dt>this.y (not shared) / this.points[i].y (shared)</dt> <dd>The y value.</dd>  </dl>
		/// </summary>
		[JsonFormatter("{0}")]
		public string Formatter { get; set; }

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
		/// <p>A callback function to place the tooltip in a default position. The callback receives three parameters: <code>labelWidth</code>, <code>labelHeight</code> and <code>point</code>, where point contains values for <code>plotX</code> and <code>plotY</code> telling where the reference point is in the plot area. Add <code>chart.plotLeft</code> and <code>chart.plotTop</code> to get the full coordinates.</p><p>The return should be an object containing x and y values, for example <code>{ x: 100, y: 100 }</code>.</p>
		/// </summary>
		[JsonFormatter("{0}")]
		public string Positioner { get; set; }

		/// <summary>
		/// Whether to apply a drop shadow to the tooltip.
		/// Default: true
		/// </summary>
		public bool? Shadow { get; set; }

		/// <summary>
		/// When the tooltip is shared, the entire plot area will capture mouse movement. Tooltip texts for series types with ordered data (not pie, scatter, flags etc) will be shown in a single bubble. This is recommended for single series charts and for tablet/mobile optimized charts.
		/// Default: false
		/// </summary>
		public bool? Shared { get; set; }

		/// <summary>
		/// Proximity snap for graphs or single points. Does not apply to bars, columns and pie slices. It defaults to 10 for mouse-powered devices and 25 for touch  devices.
		/// </summary>
		public Number? Snap { get; set; }

		/// <summary>
		/// CSS styles for the tooltip. The tooltip can also be styled through the CSS class <code>.highcharts-tooltip</code>. Default value:<pre>style: { color: '#333333', fontSize: '12px', padding: '8px'}</pre>
		/// </summary>
		[JsonFormatter("{{ {0} }}")]
		public string Style { get; set; }

		/// <summary>
		/// Use HTML to render the contents of the tooltip instead of SVG. Using HTML allows advanced formatting like tables and images in the tooltip. It is also recommended for rtl languages as it works around rtl bugs in early Firefox.
		/// Default: false
		/// </summary>
		public bool? UseHTML { get; set; }

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