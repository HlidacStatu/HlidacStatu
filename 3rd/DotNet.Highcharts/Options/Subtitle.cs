using System;
using System.Drawing;
using DotNet.Highcharts.Enums;
using DotNet.Highcharts.Attributes;
using DotNet.Highcharts.Helpers;

namespace DotNet.Highcharts.Options
{
	/// <summary>
	/// The chart's subtitle
	/// </summary>
	public class Subtitle
	{
		/// <summary>
		/// The horizontal alignment of the subtitle. Can be one of 'left', 'center' and 'right'.
		/// Default: center
		/// </summary>
		public HorizontalAligns? Align { get; set; }

		/// <summary>
		/// When the subtitle is floating, the plot area will not move to make space for it.
		/// Default: false
		/// </summary>
		public bool? Floating { get; set; }

		/// <summary>
		/// CSS styles for the title. Exact positioning of the title can be achieved by changing the margin property, or by adding <code>position: 'absolute'</code> and  left and top properties. Defaults to: <pre>{ color: '#3E576F'}</pre>
		/// </summary>
		[JsonFormatter("{{ {0} }}")]
		public string Style { get; set; }

		/// <summary>
		/// The subtitle of the chart.
		/// </summary>
		public string Text { get; set; }

		/// <summary>
		/// Whether to use HTML to render the subtitle text. Using HTML allows for advanced formatting, images and reliable bi-directional text rendering. Note that exported images won't respect the HTML, , and that HTML won't respect Z-index settings.
		/// Default: false
		/// </summary>
		public bool? UseHTML { get; set; }

		/// <summary>
		/// The vertical alignment of the title. Can be one of 'top', 'middle' and 'bottom'.
		/// Default: top
		/// </summary>
		public VerticalAligns? VerticalAlign { get; set; }

		/// <summary>
		/// The x position of the subtitle relative to the alignment within chart.spacingLeft and chart.spacingRight.
		/// Default: 0
		/// </summary>
		public Number? X { get; set; }

		/// <summary>
		/// The y position of the subtitle relative to the alignment within chart.spacingTop and chart.spacingBottom.
		/// Default: 30
		/// </summary>
		public Number? Y { get; set; }

	}

}