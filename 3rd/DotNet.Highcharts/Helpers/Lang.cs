using System.Globalization;

namespace DotNet.Highcharts.Helpers
{
    /// <summary>
    /// Specific Lang class that uses .NET Framework globalization 
    /// </summary>
    public class Lang
    {
        string _DecimalPoint;
        string[] _Months;
        string[] _ShortMonths;
        string _ThousandsSep;
        CultureInfo _UseCulture;
        string[] _Weekdays;

        /// <summary>
        /// The default decimal point used in the Highcharts.numberFormat method unless otherwise specified in the function arguments. Defaults to ".".
        /// </summary>
        /// <see cref="http://www.highcharts.com/ref/#lang--decimalPoint"/>
        public string DecimalPoint { get { return _UseCulture != null ? _UseCulture.NumberFormat.CurrencyDecimalSeparator : _DecimalPoint; } set { _DecimalPoint = value; } }

        /// <summary>
        /// An array containing the months names. Corresponds to the %B format in Highcharts.dateFormat(). 
        /// Defaults to ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'].
        /// </summary>
        /// <see cref="http://www.highcharts.com/ref/#lang--months"/>
        public string[] Months { get { return _UseCulture != null ? _UseCulture.DateTimeFormat.MonthNames : _Months; } set { _Months = value; } }

        /// <summary>
        /// An array containing the months names in abbreviated form. Corresponds to the %b format in Highcharts.dateFormat(). 
        /// Defaults to ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'].
        /// </summary>
        /// <see cref="http://www.highcharts.com/ref/#lang--shortMonths"/>
        public string[] ShortMonths { get { return _UseCulture != null ? _UseCulture.DateTimeFormat.AbbreviatedMonthNames : _ShortMonths; } set { _ShortMonths = value; } }

        /// <summary>
        /// The default thousands separator used in the Highcharts.numberFormat method unless otherwise specified in the function arguments. Defaults to ",".
        /// </summary>
        /// <see cref="http://www.highcharts.com/ref/#lang--thousandsSep"/>
        public string ThousandsSep { get { return _UseCulture != null ? _UseCulture.NumberFormat.NumberGroupSeparator : _ThousandsSep; } set { _ThousandsSep = value; } }

        /// <summary>
        /// An array containing the weekday names. Defaults to ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'].
        /// </summary>
        /// <see cref="http://www.highcharts.com/ref/#lang--weekdays"/>
        public string[] Weekdays { get { return _UseCulture != null ? _UseCulture.DateTimeFormat.DayNames : _Weekdays; } set { _Weekdays = value; } }

        /// <summary>
        /// Exporting module only. The text for the PNG download menu item. Defaults to "Download PNG image".
        /// </summary>
        /// <see cref="http://www.highcharts.com/ref/#lang--downloadPNG"/>
        public string DownloadPNG { get; set; }

        /// <summary>
        /// Exporting module only. The text for the JPEG download menu item. Defaults to "Download JPEG image".
        /// </summary>
        /// <see cref="http://www.highcharts.com/ref/#lang--downloadJPEG"/>
        public string DownloadJPEG { get; set; }

        /// <summary>
        /// Exporting module only. The text for the PDF download menu item. Defaults to "Download PDF document".
        /// </summary>
        /// <see cref="http://www.highcharts.com/ref/#lang--downloadPDF"/>
        public string DownloadPDF { get; set; }

        /// <summary>
        /// Exporting module only. The text for the SVG download menu item. Defaults to "Download SVG vector image".
        /// </summary>
        /// <see cref="http://www.highcharts.com/ref/#lang--downloadSVG"/>
        public string DownloadSVG { get; set; }

        /// <summary>
        /// Exporting module only. The tooltip text for the export button. Defaults to "Export to raster or vector image".
        /// </summary>
        /// <see cref="http://www.highcharts.com/ref/#lang--exportButtonTitle"/>
        public string ExportButtonTitle { get; set; }

        /// <summary>
        /// The loading text that appears when the chart is set into the loading state following a call to chart.showLoading. Defaults to Loading....
        /// </summary>
        /// <see cref="http://www.highcharts.com/ref/#lang--loading"/>
        public string Loading { get; set; }

        /// <summary>
        /// Exporting module only. The tooltip text for the print button. Defaults to "Print the chart".
        /// </summary>
        /// <see cref="http://www.highcharts.com/ref/#lang--printButtonTitle"/>
        public string PrintButtonTitle { get; set; }

        /// <summary>
        /// The text for the label appearing when a chart is zoomed. Defaults to Reset zoom.
        /// </summary>
        /// <see cref="http://www.highcharts.com/ref/#lang--resetZoom"/>
        public string ResetZoom { get; set; }

        /// <summary>
        /// The tooltip title for the label appearing when a chart is zoomed. Defaults to Reset zoom level 1:1.
        /// </summary>
        /// <see cref="http://www.highcharts.com/ref/#lang--resetZoomTitle"/>
        public string ResetZoomTitle { get; set; }

        /// <summary>
        /// Set culture by string and will be used in DecimalPoint, Months, ShortMonths, ThausandsSep and Weekdays.
        /// </summary>
        /// <param name="culture">Culture name like en-US</param>
        /// <returns></returns>
        public Lang SetAndUseCulture(string culture)
        {
            _UseCulture = new CultureInfo(culture);
            return this;
        }
    }
}