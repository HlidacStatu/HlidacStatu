using System;
using System.Drawing;
using DotNet.Highcharts.Enums;
using DotNet.Highcharts.Attributes;
using DotNet.Highcharts.Helpers;

namespace DotNet.Highcharts.Options
{
	/// <summary>
	/// HTML labels that can be positioined anywhere in the chart area.
	/// </summary>
	public class Labels
	{
		/// <summary>
		/// A HTML label that can be positioined anywhere in the chart area.
		/// </summary>
		public LabelsItems[] Items { get; set; }

		/// <summary>
		/// Shared CSS styles for all labels. Defaults to:<pre>style: { color: '#3E576F'}</pre>
		/// </summary>
		[JsonFormatter("{{ {0} }}")]
		public string Style { get; set; }

	}

}