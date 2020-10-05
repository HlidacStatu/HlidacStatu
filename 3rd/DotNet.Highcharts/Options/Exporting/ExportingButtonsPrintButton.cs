using System.Drawing;
using DotNet.Highcharts.Attributes;
using DotNet.Highcharts.Helpers;

namespace DotNet.Highcharts.Options.Exporting
{
	/// <summary>
	/// Options for the print button.
	/// </summary>
	public class ExportingButtonsPrintButton : NavigationButtonOptions
	{
		/// <summary>
		/// See navigation.buttonOptions
		/// => hoverSymbolFill. Defaults to #779ABF
		/// </summary>
		public Color? HoverSymbolFill { get; set; }

		/// <summary>
		/// A click handler callback to use on the button directly. By default this onclick calls
		/// chart.print(), but it can be overridden to do other actions.
		/// </summary>
		[JsonFormatter("{0}")]
		public string Onclick { get; set; }

		/// <summary>
		/// The symbol for the button. Points to a definition function in the 
		/// Highcharts.Renderer.symbols collection. The default
		/// print function is part of the exporting module. Defaults to "printIcon"
		/// </summary>
		public string Symbol { get; set; }

		/// <summary>
		/// The horizontal positioin of the button relative to the align
		/// option. Defaults to -36
		/// </summary>
		public Number? X { get; set; }

	}

}