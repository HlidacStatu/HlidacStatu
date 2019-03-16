using System;
using System.Drawing;
using DotNet.Highcharts.Attributes;
using DotNet.Highcharts.Enums;
using DotNet.Highcharts.Helpers;

namespace DotNet.Highcharts.Options.Exporting
{
	/// <summary>
	/// Options for the export button.
	/// </summary>
	public class ExportingButtonsContextButton
	{
		/// <summary>
		/// Alignment for the buttons.
		/// Default: right
		/// </summary>
		public HorizontalAligns? Align { get; set; }

		/// <summary>
		/// Whether to enable buttons.
		/// Default: true
		/// </summary>
		public bool? Enabled { get; set; }

		/// <summary>
		/// Pixel height of the buttons.
		/// Default: 20
		/// </summary>
		public Number? Height { get; set; }

		/// <summary>
		/// <p>A collection of config options for the menu items. Each options object consists of a <code>text</code> option which is a string to show in the menu item, as well as an <code>onclick</code> parameter which is a callback function to run on click.</p> <p>By default, there is the 'Print' menu item plus one menu item for each of the available export types. Menu items can be customized by defining a new array of items and assigning  <code>null</code> to unwanted positions (see override example below).</p>
		/// </summary>
		public MenuItem[] MenuItems { get; set; }

		/// <summary>
		/// A click handler callback to use on the button directly instead of the popup menu.
		/// </summary>
		[JsonFormatter("{0}")]
		public string Onclick { get; set; }

		/// <summary>
		/// The symbol for the button. Points to a definition function in the  <code>Highcharts.Renderer.symbols</code> collection. The default <code>exportIcon</code> function is part of the exporting module.
		/// Default: menu
		/// </summary>
		public string Symbol { get; set; }

		/// <summary>
		/// See <a class='internal' href='#navigation-buttonOptions'>navigation.buttonOptions</a> =&gt; symbolFill.
		/// Default: #A8BF77
		/// </summary>
		public Color? SymbolFill { get; set; }

		/// <summary>
		/// The pixel size of the symbol on the button.
		/// Default: 14
		/// </summary>
		public Number? SymbolSize { get; set; }

		/// <summary>
		/// The color of the symbol's stroke or line.
		/// Default: #666
		/// </summary>
		public Color? SymbolStroke { get; set; }

		/// <summary>
		/// The pixel stroke width of the symbol on the button.
		/// Default: 1
		/// </summary>
		public Number? SymbolStrokeWidth { get; set; }

		/// <summary>
		/// The x position of the center of the symbol inside the button.
		/// Default: 12.5
		/// </summary>
		public Number? SymbolX { get; set; }

		/// <summary>
		/// The y position of the center of the symbol inside the button.
		/// Default: 10.5
		/// </summary>
		public Number? SymbolY { get; set; }

		/// <summary>
		/// A text string to add to the individual button. 
		/// Default: null
		/// </summary>
		public string Text { get; set; }

		/// <summary>
		/// A configuration object for the button theme. The object accepts SVG properties like <code>stroke-width</code>, <code>stroke</code> and <code>fill</code>. Tri-state button styles are supported by the <code>states.hover</code> and <code>states.select</code> objects.
		/// </summary>
		public Object Theme { get; set; }

		/// <summary>
		/// The vertical alignment of the buttons. Can be one of 'top', 'middle' or 'bottom'.
		/// Default: top
		/// </summary>
		public VerticalAligns? VerticalAlign { get; set; }

		/// <summary>
		/// The pixel width of the button.
		/// Default: 24
		/// </summary>
		public Number? Width { get; set; }

		/// <summary>
		/// The horizontal positioin of the button relative to the <code>align</code> option.
		/// Default: -10
		/// </summary>
		public Number? X { get; set; }

		/// <summary>
		/// The vertical offset of the button's position relative to its <code>verticalAlign</code>. .
		/// Default: 0
		/// </summary>
		public Number? Y { get; set; }

	}

}