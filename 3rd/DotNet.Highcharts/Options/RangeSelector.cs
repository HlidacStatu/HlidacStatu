using System;
using System.Drawing;
using DotNet.Highcharts.Enums;
using DotNet.Highcharts.Attributes;
using DotNet.Highcharts.Helpers;

/// TODO: buttonTheme, buttons,inputPosition, inputStyle, labelStyle.
namespace DotNet.Highcharts.Options
{
	/// <summary>
    /// The range selector is a tool for selecting ranges to display within the chart.
    /// It provides buttons to select preconfigured ranges in the chart,
    /// like 1 day, 1 week, 1 month etc. It also provides input boxes
    /// where min and max dates can be manually input.
	/// </summary>
	public class RangeSelector
	{
		/// <summary>
        /// The space in pixels between the buttons in the range selector. Defaults to 0.
		/// </summary>
		public Number? ButtonSpacing { get; set; }

        /// <summary>
        /// Enable or disable the range selector. Defaults to true.
        /// </summary>
        public bool? Enabled { get; set; }

        /// <summary>
        /// The date format in the input boxes when not selected for editing.
        /// Defaults to %b %e, %Y. Defaults to %b %e %Y.
        /// </summary>
        public string InputDateFormat { get; set; }

        /// <summary>
        /// The date format in the input boxes when they are selected for editing.
        /// This must be a format that is recognized by JavaScript Date.parse.
        /// Defaults to %Y-%m-%d.
        /// </summary>
        public string InputEditDateFormat { get; set; }

        /// <summary>
        /// Enable or disable the date input boxes. Defaults to true.
        /// </summary>
        public bool? InputEnabled { get; set; }

        /// <summary>
        /// The index of the button to appear pre-selected. Defaults to undefined.
        /// </summary>
        public Number? Selected { get; set; }
	}
}