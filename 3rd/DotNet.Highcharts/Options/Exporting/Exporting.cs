using System;
using DotNet.Highcharts.Helpers;

namespace DotNet.Highcharts.Options.Exporting
{
	/// <summary>
	/// Options for the exporting module. For an overview on the matter, see <a href='http://docs.highcharts.com/#export-module'>the docs</a>.
	/// </summary>
	public class Exporting
	{
		/// <summary>
		/// Options for the export related buttons, print and export. In addition to the default buttons listed here, custom buttons can be added. See <a href='#navigation.buttonOptions'>navigation.buttonOptions</a> for general options.
		/// </summary>
		public ExportingButtons Buttons { get; set; }

		/// <summary>
		/// Additional chart options to be merged into an exported chart. For example, the exported chart can be given a specific width and height, or a printer-friendly color scheme.
		/// Default: null
		/// </summary>
		public Object ChartOptions { get; set; }

		/// <summary>
		/// Whether to enable the exporting module.
		/// Default: true
		/// </summary>
		public bool? Enabled { get; set; }

		/// <summary>
		/// The filename, without extension, to use for the exported chart.
		/// Default: chart
		/// </summary>
		public string Filename { get; set; }

		/// <summary>
		/// Defines the scale or zoom factor for the exported image compared to the on-screen display. While for instance a 600px wide chart may look good on a website, it will look bad in print. The default scale of 2 makes this chart export to a 1200px PNG or JPG. 
		/// Default: 2
		/// </summary>
		public Number? Scale { get; set; }

		/// <summary>
		/// Analogous to <a href='#exporting.sourceWidth'>sourceWidth</a>
		/// </summary>
		public Number? SourceHeight { get; set; }

		/// <summary>
		/// The width of the original chart when exported, unless an explicit <a href='#chart.width'>chart.width</a> is set. The width exported raster image is then multiplied by <a href='#exporting.scale'>scale</a>.
		/// </summary>
		public Number? SourceWidth { get; set; }

		/// <summary>
		/// Default MIME type for exporting if <code>chart.exportChart()</code> is called without specifying a <code>type</code> option. Possible values are <code>image/png</code>, <code>image/jpeg</code>, <code>application/pdf</code> and <code>image/svg+xml</code>.
		/// Default: image/png
		/// </summary>
		public string Type { get; set; }

		/// <summary>
		/// The URL for the server module converting the SVG string to an image format. By default this points to Highslide Software's free web service.
		/// Default: http://export.highcharts.com
		/// </summary>
		public string Url { get; set; }

		/// <summary>
		/// The pixel width of charts exported to PNG or JPG. As of Highcharts 3.0, the default pixel width is a function of the <a href='#chart.width'>chart.width</a> or <a href='#exporting.sourceWidth'>exporting.sourceWidth</a> and the <a href='#exporting.scale'>exporting.scale</a>.
		/// Default: undefined
		/// </summary>
		public Number? Width { get; set; }

	}

}