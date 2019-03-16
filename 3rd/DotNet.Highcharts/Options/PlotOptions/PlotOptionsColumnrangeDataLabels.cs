using System.Drawing;
using DotNet.Highcharts.Attributes;
using DotNet.Highcharts.Enums;
using DotNet.Highcharts.Helpers;

namespace DotNet.Highcharts.Options.PlotOptions
{
	/// <summary>
	/// Extended data labels for range series types. Range series  data labels have no <code>x</code> and <code>y</code> options. Instead, they have <code>xLow</code>, <code>xHigh</code>, <code>yLow</code> and <code>yHigh</code> options to allow the higher and lower data label sets individually. 
	/// </summary>
	public class PlotOptionsColumnrangeDataLabels
	{
		/// <summary>
		/// The alignment of the data label compared to the point. Can be  one of 'left', 'center' or 'right'. Defaults to <code>'center'</code>.
		/// Default: center
		/// </summary>
		public HorizontalAligns? Align { get; set; }

		/// <summary>
		/// The background color or gradient for the data label. Defaults to <code>undefined</code>.
		/// </summary>
		[JsonFormatter(addPropertyName: true, useCurlyBracketsForObject: false)]
		public BackColorOrGradient BackgroundColor { get; set; }

		/// <summary>
		/// The border color for the data label. Defaults to <code>undefined</code>.
		/// </summary>
		public Color? BorderColor { get; set; }

		/// <summary>
		/// The border radius in pixels for the data label. Defaults to <code>0</code>.
		/// Default: 0
		/// </summary>
		public Number? BorderRadius { get; set; }

		/// <summary>
		/// The border width in pixels for the data label. Defaults to <code>0</code>.
		/// Default: 0
		/// </summary>
		public Number? BorderWidth { get; set; }

		/// <summary>
		/// The text color for the data labels. Defaults to <code>null</code>.
		/// </summary>
		public Color? Color { get; set; }

		/// <summary>
		/// Whether to hide data labels that are outside the plot area. By default, a data label only shows if the point or the data label is within the plot area.
		/// Default: true
		/// </summary>
		public bool? Crop { get; set; }

		/// <summary>
		/// Enable or disable the data labels. Defaults to <code>false</code>.
		/// Default: false
		/// </summary>
		public bool? Enabled { get; set; }

		/// <summary>
		/// A <a href='http://docs.highcharts.com/#formatting'>format string</a> for the data label. Available variables are the same as for <code>formatter</code>.
		/// Default: {y}
		/// </summary>
		public string Format { get; set; }

		/// <summary>
		/// Callback JavaScript function to format the data label. Available data are:<table><tbody><tr>  <td><code>this.percentage</code></td>  <td>Stacked series and pies only. The point's percentage of the total.</td></tr><tr>  <td><code>this.point</code></td>  <td>The point object. The point name, if defined, is available through <code>this.point.name</code>.</td></tr><tr>  <td><code>this.series</code>:</td>  <td>The series object. The series name is available through <code>this.series.name</code>.</td></tr><tr>  <td><code>this.total</code></td>  <td>Stacked series only. The total value at this point's x value.</td></tr><tr>  <td><code>this.x</code>:</td>  <td>The y value.</td></tr><tr>  <td><code>this.y</code>:</td>  <td>The y value.</td></tr></tbody></table>
		/// </summary>
		[JsonFormatter("{0}")]
		public string Formatter { get; set; }

		/// <summary>
		/// For points with an extent, like columns, whether to align the data label inside the box or to the actual value point. Defaults to <code>false</code> in most cases, <code>true</code> in stacked columns.
		/// </summary>
		public bool? Inside { get; set; }

		/// <summary>
		/// When either the <code>borderWidth</code> or the <code>backgroundColor</code> is set, this is the padding within the box. Defaults to <code>2</code>.
		/// Default: 2
		/// </summary>
		public Number? Padding { get; set; }

		/// <summary>
		/// Text rotation in degrees. Defaults to <code>0</code>.
		/// Default: 0
		/// </summary>
		public Number? Rotation { get; set; }

		/// <summary>
		/// The shadow of the box. Works best with <code>borderWidth</code> or <code>backgroundColor</code>. Since 2.3 the shadow can be an object configuration containing <code>color</code>, <code>offsetX</code>, <code>offsetY</code>, <code>opacity</code> and <code>width</code>.
		/// Default: false
		/// </summary>
		public bool? Shadow { get; set; }

		/// <summary>
		/// Styles for the label.
		/// </summary>
		[JsonFormatter("{{ {0} }}")]
		public string Style { get; set; }

		/// <summary>
		/// Whether to use HTML to render the labels. Using HTML allows advanced formatting, images and reliable bi-directional text rendering. Note that exported images won't respect the HTML, and that HTML won't respect Z-index settings.
		/// Default: false
		/// </summary>
		public bool? UseHTML { get; set; }

		/// <summary>
		/// The vertical alignment of a data label. Can be one of <code>top</code>, <code>middle</code> or <code>bottom</code>. The default value depends on the data, for instance in a column chart, the label is above positive values and below negative values.
		/// </summary>
		public VerticalAligns? VerticalAlign { get; set; }

		/// <summary>
		/// X offset of the higher data labels relative to the point value.
		/// Default: 0
		/// </summary>
		public Number? XHigh { get; set; }

		/// <summary>
		/// X offset of the lower data labels relative to the point value.
		/// Default: 0
		/// </summary>
		public Number? XLow { get; set; }

		/// <summary>
		/// Y offset of the higher data labels relative to the point value.
		/// Default: -6
		/// </summary>
		public Number? YHigh { get; set; }

		/// <summary>
		/// Y offset of the lower data labels relative to the point value.
		/// Default: 16
		/// </summary>
		public Number? YLow { get; set; }

		/// <summary>
		/// The Z index of the data labels. The default Z index puts it above the series. Use a Z index of 2 to display it behind the series.
		/// Default: 6
		/// </summary>
		public Number? ZIndex { get; set; }

	}

}