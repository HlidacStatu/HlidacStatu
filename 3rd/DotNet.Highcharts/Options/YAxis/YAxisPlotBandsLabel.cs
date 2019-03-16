using DotNet.Highcharts.Attributes;
using DotNet.Highcharts.Enums;
using DotNet.Highcharts.Helpers;

namespace DotNet.Highcharts.Options.YAxis
{
	/// <summary>
	/// Text labels for the plot bands
	/// </summary>
	public class YAxisPlotBandsLabel
	{
		/// <summary>
		/// Horizontal alignment of the label. Can be one of 'left', 'center' or 'right'.
		/// Default: center
		/// </summary>
		public HorizontalAligns? Align { get; set; }

		/// <summary>
		/// Rotation of the text label in degrees .
		/// Default: 0
		/// </summary>
		public Number? Rotation { get; set; }

		/// <summary>
		/// CSS styles for the text label.
		/// </summary>
		[JsonFormatter("{{ {0} }}")]
		public string Style { get; set; }

		/// <summary>
		/// The string text itself. A subset of HTML is supported.
		/// </summary>
		public string Text { get; set; }

		/// <summary>
		/// The text alignment for the label. While <code>align</code> determines where the texts anchor point is placed within the plot band, <code>textAlign</code> determines how the text is aligned against its anchor point. Possible values are 'left', 'center' and 'right'. Defaults to the same as the <code>align</code> option.
		/// </summary>
		public HorizontalAligns? TextAlign { get; set; }

        /// <summary>
        /// Whether to use HTML to render the labels.
        /// Default: false
        /// </summary>
        public bool? UseHTML { get; set; }

		/// <summary>
		/// Vertical alignment of the label relative to the plot band. Can be one of 'top', 'middle' or 'bottom'.
		/// Default: top
		/// </summary>
		public VerticalAligns? VerticalAlign { get; set; }

		/// <summary>
		/// Horizontal position relative the alignment. Default varies by orientation.
		/// </summary>
		public Number? X { get; set; }

		/// <summary>
		/// Vertical position of the text baseline relative to the alignment. Default varies by orientation.
		/// </summary>
		public Number? Y { get; set; }

	}

}