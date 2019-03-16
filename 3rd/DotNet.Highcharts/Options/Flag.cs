using System;
using System.Drawing;
using DotNet.Highcharts.Enums;
using DotNet.Highcharts.Attributes;
using DotNet.Highcharts.Helpers;

namespace DotNet.Highcharts.Options
{
	/// <summary>
	/// Config options for the individual flag.
	/// </summary>
	public class Flag
	{
        /// <summary>
        /// The text displayed when the flag are highlighted.
        /// </summary>
        public string Text { get; set; }

		/// <summary>
        /// The title of flag displayed on the chart.
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// Pie series only. Whether to display a slice offset from the center. Defaults to false
		/// </summary>
		public bool? Sliced { get; set; }

		/// <summary>
        /// The x point where the flag appears.
		/// </summary>
		public Number? X { get; set; }
	}
}