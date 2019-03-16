using DotNet.Highcharts.Attributes;
using DotNet.Highcharts.Enums;
using DotNet.Highcharts.Helpers;

namespace DotNet.Highcharts.Options.YAxis
{
	/// <summary>
	/// The stack labels show the total value for each bar in a stacked column or bar chart. The label will be placed on top of positive columns and below negative columns. In case of an inverted column chart or a bar chart the label is placed to  the right of positive bars and to the left of negative bars.
	/// </summary>
	public class YAxisStackLabels
	{
		/// <summary>
		/// Defines the horizontal alignment of the stack total label.  Can be one of <code>'left'</code>, <code>'center'</code> or <code>'right'</code>. The default value is calculated at runtime and depends on orientation and whether  the stack is positive or negative.
		/// </summary>
		public HorizontalAligns? Align { get; set; }

        /// <summary>
        /// Allow the stack labels to overlap.
        /// Default: false
        /// </summary>
        public bool? AllowOverlap { get; set; }

		/// <summary>
		/// Enable or disable the stack total labels.
		/// Default: false
		/// </summary>
		public bool? Enabled { get; set; }

        /// <summary>
        /// A format string for the data label. Available variables are the same as for formatter.
        /// Default: {total}
        /// </summary>
        public string Format { get; set; }

		/// <summary>
		/// Callback JavaScript function to format the label. The value is  given by <code>this.total</code>. Defaults to: <pre>function() { return this.total;}</pre>
		/// </summary>
		[JsonFormatter("{0}")]
		public string Formatter { get; set; }

		/// <summary>
		/// Rotation of the labels in degrees.
		/// Default: 0
		/// </summary>
		public Number? Rotation { get; set; }

		/// <summary>
		/// CSS styles for the label. Defaults to:<pre>style: { color: '#666', 'font-size': '11px', 'line-height': '14px'}</pre>
		/// </summary>
		[JsonFormatter("{{ {0} }}")]
		public string Style { get; set; }

		/// <summary>
		/// The text alignment for the label. While <code>align</code> determines where the texts anchor point is placed with regards to the stack, <code>textAlign</code> determines how the text is aligned against its anchor point. Possible values are <code>'left'</code>, <code>'center'</code> and <code>'right'</code>. The default value is calculated at runtime and depends on orientation and whether the stack is positive or negative.
		/// </summary>
		public HorizontalAligns? TextAlign { get; set; }

		/// <summary>
		/// Whether to use HTML for the stack labels. Using HTML allows for advanced formatting, images and reliable bi-directional text rendering. Note that exported images won't respect the HTML, and that HTML won't respect Z-index settings.
		/// Default: false
		/// </summary>
		public bool? UseHTML { get; set; }

		/// <summary>
		/// Defines the vertical alignment of the stack total label. Can be one of <code>'top'</code>, <code>'middle'</code> or <code>'bottom'</code>. The default value is calculated at runtime and depends on orientation and whether  the stack is positive or negative.
		/// </summary>
		public VerticalAligns? VerticalAlign { get; set; }

		/// <summary>
		/// The x position offset of the label relative to the left of the stacked bar. The default value is calculated at runtime and depends on orientation and whether the stack is positive or negative.
		/// </summary>
		public Number? X { get; set; }

		/// <summary>
		/// The y position offset of the label relative to the tick position on the axis. The default value is calculated at runtime and depends on orientation and whether  the stack is positive or negative.
		/// </summary>
		public Number? Y { get; set; }

	}

}