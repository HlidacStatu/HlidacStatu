using System;
using System.Drawing;
using DotNet.Highcharts.Enums;
using DotNet.Highcharts.Attributes;
using DotNet.Highcharts.Helpers;

namespace DotNet.Highcharts.Options
{
	/// <summary>
	/// A HTML label that can be positioined anywhere in the chart area.
	/// </summary>
	public class LabelsItems
	{
		/// <summary>
		/// Inner HTML or text for the label.
		/// </summary>
		public string Html { get; set; }

		/// <summary>
		/// CSS styles for each label. To position the label, use left and top like this:<pre>style: { left: '100px', top: '100px'}</pre>
		/// </summary>
		[JsonFormatter("{{ {0} }}")]
		public string Style { get; set; }

	}

}