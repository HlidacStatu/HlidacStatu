using System;
using System.Drawing;
using DotNet.Highcharts.Enums;
using DotNet.Highcharts.Attributes;
using DotNet.Highcharts.Helpers;

namespace DotNet.Highcharts.Options
{
	/// <summary>
	/// Applies only to polar charts and angular gauges. This configuration object holds general options for the combined X and Y axes set. Each xAxis or yAxis can reference the pane by index.
	/// </summary>
	public class Pane
	{
		/// <summary>
		/// An object, or array of objects, for backgrounds. Sub options include <code>backgroundColor</code> (which can be solid or gradient), <code>innerWidth</code>, <code>outerWidth</code>, <code>borderWidth</code>, <code>borderColor</code>.
		/// </summary>
		public BackgroundObject[] Background { get; set; }

		/// <summary>
		/// The center of a polar chart or angular gauge, given as an array of [x, y] positions. Positions can be given as integers that transform to pixels, or as percentages of the plot area size. Defaults to ['50%', '50%'].
		/// </summary>
		public PercentageOrPixel[] Center { get; set; }

		/// <summary>
		/// The end angle of the polar X axis or gauge value axis, given in degrees where 0 is north. Defaults to <a href='#pane.startAngle'>startAngle</a> + 360.
		/// </summary>
		public Number? EndAngle { get; set; }

		/// <summary>
		/// The start angle of the polar X axis or gauge axis, given in degrees where 0 is north. Defaults to 0.
		/// </summary>
		public Number? StartAngle { get; set; }

		[JsonFormatter(addPropertyName: true, useCurlyBracketsForObject: false)]
		public PercentageOrPixel Size { get; set; }
	}

}