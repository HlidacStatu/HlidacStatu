using System;
using System.Drawing;
using DotNet.Highcharts.Enums;
using DotNet.Highcharts.Attributes;
using DotNet.Highcharts.Helpers;

namespace DotNet.Highcharts.Options
{
	/// <summary>
	/// The button that appears after a selection zoom, allowing the user to reset zoom.
	/// </summary>
	public class ChartResetZoomButton
	{
		/// <summary>
		/// The position of the button.
		/// </summary>
		public Position Position { get; set; }

		/// <summary>
		/// What frame the button should be placed related to. Can be either 'plot' or 'chart'.
		/// Default: plot
		/// </summary>
		public RelativeTo? RelativeTo { get; set; }

		/// <summary>
		/// A collection of attributes for the button. The object takes SVG attributes like  <code>fill</code>, <code>stroke</code>, <code>stroke-width</code> or <code>r</code>, the border radius. The theme also supports <code>style</code>, a collection of CSS properties for the text. Equivalent attributes for the hover state are given in <code>theme.states.hover</code>.
		/// </summary>
		public Theme Theme { get; set; }

	}

}