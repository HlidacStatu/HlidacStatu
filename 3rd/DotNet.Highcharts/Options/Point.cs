using System;
using System.Drawing;
using DotNet.Highcharts.Enums;
using DotNet.Highcharts.Attributes;
using DotNet.Highcharts.Helpers;
using DotNet.Highcharts.Options.PlotOptions;

namespace DotNet.Highcharts.Options
{
	/// <summary>
	/// Config options for the individual point as given in series.data.
	/// </summary>
	public class Point
	{
		/// <summary>
		/// Individual color for the point. Defaults to null
		/// </summary>
		public Color? Color { get; set; }

		/// <summary>
		/// Individual data label for each point. The options are the same as the ones for 
		/// plotOptions.series.dataLabels
		/// </summary>
		public Object DataLabels { get; set; }

		/// <summary>
		/// Individual events for the point. Defaults to null
		/// </summary>
		public PlotOptionsSeriesPointEvents Events { get; set; }

		/// <summary>
		/// An id for the point. This can be used after render time to get a pointer
		/// to the point object through chart.get(). Defaults to null
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// An individual point marker for the point. Defaults to null
		/// </summary>
		public PlotOptionsSeriesMarker Marker { get; set; }

		/// <summary>
		/// Pies only. The sequential index of the pie slice in the legend. Defaults to undefined
		/// </summary>
		public Number? LegendIndex { get; set; }

		/// <summary>
		/// The name of the point as shown in the legend, tooltip, dataLabel etc. Defaults to ""
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Pie series only. Whether to display a slice offset from the center. Defaults to false
		/// </summary>
		public bool? Sliced { get; set; }

		/// <summary>
		/// The x value of the point Defaults to null
		/// </summary>
		public Object X { get; set; }

		/// <summary>
		/// The y value of the point Defaults to null
		/// </summary>
        public Object Y { get; set; }

		public Drilldown Drilldown { get; set; }

		public bool? Selected { get; set; }

        public bool? IsIntermediateSum { get; set; }
        
        public bool? IsSum { get; set; }

        // Additional Properties
        public Object ViewY { get; set; }
        public Object DeltaY { get; set; }
	}

}