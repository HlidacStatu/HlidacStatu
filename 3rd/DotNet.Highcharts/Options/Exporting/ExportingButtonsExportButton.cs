using System.Drawing;
using DotNet.Highcharts.Attributes;
using DotNet.Highcharts.Helpers;

namespace DotNet.Highcharts.Options.Exporting
{
	/// <summary>
	/// Options for the export button.
	/// </summary>
	public class ExportingButtonsExportButton : NavigationButtonOptions
	{
		/// <summary>
		/// See navigation.buttonOptions
		/// => hoverSymbolFill. Defaults to #768F3E
		/// </summary>
		public Color? HoverSymbolFill { get; set; }

		/// <summary>
		/// A click handler callback to use on the button directly instead of the popup menu.
		/// </summary>
		[JsonFormatter("{0}")]
		public string Onclick { get; set; }

		/// <summary>
		/// A collection of config options for the menu items. Each options object consists
		/// of a text option which is a string to show in the menu item, as
		/// well as an onclick parameter which is a callback function to run
		/// on click.
		/// By default, there is one menu item for each of the available export types.
		/// Menu items can be customized by defining a new array of items and assigning 
		/// null to unwanted positions (see override example below).
		/// </summary>
		public MenuItem[] MenuItems { get; set; }

		/// <summary>
		/// The symbol for the button. Points to a definition function in the 
		/// Highcharts.Renderer.symbols collection. The default
		/// exportIcon function is part of the exporting module. Defaults to "exportIcon"
		/// </summary>
		public string Symbol { get; set; }

		/// <summary>
		/// See navigation.buttonOptions
		/// => symbolFill. Defaults to #A8BF77
		/// </summary>
		public Color? SymbolFill { get; set; }

		/// <summary>
		/// The horizontal positioin of the button relative to the align
		/// option. Defaults to 10
		/// </summary>
		public Number? X { get; set; }

	}

}