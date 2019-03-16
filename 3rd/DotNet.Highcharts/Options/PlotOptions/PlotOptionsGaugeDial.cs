using System.Drawing;
using DotNet.Highcharts.Attributes;
using DotNet.Highcharts.Helpers;

namespace DotNet.Highcharts.Options.PlotOptions
{
	/// <summary>
	/// Options for the dial or arrow pointer of the gauge.
	/// </summary>
	public class PlotOptionsGaugeDial
	{
		/// <summary>
		/// The background or fill color of the gauge's dial. 
		/// Default: black
		/// </summary>
		[JsonFormatter(addPropertyName: true, useCurlyBracketsForObject: false)]
		public BackColorOrGradient BackgroundColor { get; set; }

		/// <summary>
		/// The length of the dial's base part, relative to the total radius or length of the dial. 
		/// Default: 70%
		/// </summary>
		public string BaseLength { get; set; }

		/// <summary>
		/// The pixel width of the base of the gauge dial. The base is the part closest to the pivot, defined by baseLength. 
		/// Default: 3
		/// </summary>
		public Number? BaseWidth { get; set; }

		/// <summary>
		/// The border color or stroke of the gauge's dial. By default, the borderWidth is 0, so this must be set in addition to a custom border color.
		/// Default: silver
		/// </summary>
		public Color? BorderColor { get; set; }

		/// <summary>
		/// The width of the gauge dial border in pixels.
		/// Default: 0
		/// </summary>
		public Number? BorderWidth { get; set; }

		/// <summary>
		/// The radius or length of the dial, in percentages relative to the radius of the gauge itself.
		/// Default: 80%
		/// </summary>
		public string Radius { get; set; }

		/// <summary>
		/// The length of the dial's rear end, the part that extends out on the other side of the pivot. Relative to the dial's length. 
		/// Default: 10%
		/// </summary>
		public string RearLength { get; set; }

		/// <summary>
		/// The width of the top of the dial, closest to the perimeter. The pivot narrows in from the base to the top.
		/// Default: 1
		/// </summary>
		public Number? TopWidth { get; set; }

	}

}