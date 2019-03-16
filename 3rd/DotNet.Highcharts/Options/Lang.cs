using System;
using System.Drawing;
using DotNet.Highcharts.Enums;
using DotNet.Highcharts.Attributes;
using DotNet.Highcharts.Helpers;

namespace DotNet.Highcharts.Options
{
	/// <summary>
	/// Language object. The language object is global and it can't be set on each chart initiation. Instead, use <code>Highcharts.setOptions</code> to set it before any chart is initiated. <pre>Highcharts.setOptions({ lang: { months: ['Janvier', 'Février', 'Mars', 'Avril', 'Mai', 'Juin',  'Juillet', 'Août', 'Septembre', 'Octobre', 'Novembre', 'Décembre'], weekdays: ['Dimanche', 'Lundi', 'Mardi', 'Mercredi', 'Jeudi', 'Vendredi', 'Samedi'] }});</pre>
	/// </summary>
	public class Lang
	{
		/// <summary>
		/// Exporting module menu. The tooltip title for the context menu holding print and export menu items.
		/// Default: Chart context menu
		/// </summary>
		public string ContextButtonTitle { get; set; }

		/// <summary>
		/// The default decimal point used in the <code>Highcharts.numberFormat</code> method unless otherwise specified in the function arguments.
		/// Default: .
		/// </summary>
		public string DecimalPoint { get; set; }

		/// <summary>
		/// Exporting module only. The text for the JPEG download menu item.
		/// Default: Download JPEG image
		/// </summary>
		public string DownloadJPEG { get; set; }

		/// <summary>
		/// Exporting module only. The text for the PDF download menu item.
		/// Default: Download PDF document
		/// </summary>
		public string DownloadPDF { get; set; }

		/// <summary>
		/// Exporting module only. The text for the PNG download menu item.
		/// Default: Download PNG image
		/// </summary>
		public string DownloadPNG { get; set; }

		/// <summary>
		/// Exporting module only. The text for the SVG download menu item.
		/// Default: Download SVG vector image
		/// </summary>
		public string DownloadSVG { get; set; }

		/// <summary>
		/// The loading text that appears when the chart is set into the loading state following a call to <code>chart.showLoading</code>.
		/// Default: Loading...
		/// </summary>
		public string Loading { get; set; }

		/// <summary>
		/// An array containing the months names. Corresponds to the  <code>%B</code> format in <code>Highcharts.dateFormat()</code>.
		/// </summary>
		public string[] Months { get; set; }

		/// <summary>
		/// <a href='http://en.wikipedia.org/wiki/Metric_prefix'>Metric prefixes</a> used to shorten high numbers in axis labels. Replacing any of the positions with <code>null</code> causes the full number to be written. Setting <code>numbericSymbols</code> to <code>null</code> disables shortening altogether.
		/// </summary>
		public string[] NumericSymbols { get; set; }

		/// <summary>
		/// Exporting module only. The text for the menu item to print the chart.
		/// Default: Print chart
		/// </summary>
		public string PrintChart { get; set; }

		/// <summary>
		/// The text for the label appearing when a chart is zoomed.
		/// Default: Reset zoom
		/// </summary>
		public string ResetZoom { get; set; }

		/// <summary>
		/// The tooltip title for the label appearing when a chart is zoomed.
		/// Default: Reset zoom level 1:1
		/// </summary>
		public string ResetZoomTitle { get; set; }

		/// <summary>
		/// An array containing the months names in abbreviated form. Corresponds to the  <code>%b</code> format in <code>Highcharts.dateFormat()</code>. 
		/// </summary>
		public string[] ShortMonths { get; set; }

		/// <summary>
		/// The default thousands separator used in the <code>Highcharts.numberFormat</code> method unless otherwise specified in the function arguments. Defaults to <code>','</code>.
		/// Default: ,
		/// </summary>
		public string ThousandsSep { get; set; }

		/// <summary>
		/// An array containing the weekday names. 
		/// Default: ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"]
		/// </summary>
		public string[] Weekdays { get; set; }

	}

}