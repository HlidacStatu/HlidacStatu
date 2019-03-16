using DotNet.Highcharts.Attributes;
using DotNet.Highcharts.Helpers;

namespace DotNet.Highcharts.Options.DataOptions
{
    /// <summary>
    /// The Data module provides a simplified interface for adding data to a chart from sources like CVS, HTML tables or grid views. See also the tutorial article on the Data module.
    /// It requires the modules/data.js file to be loaded.
    /// Please note that the default way of adding data in Highcharts, without the need of a module, is through the series.data option.
    /// </summary>
    public class DataOptions
    {
        /// <summary>
        /// A callback function to modify the CSV before parsing it. Return the modified string.
        /// Default: undefined
        /// </summary>
        [JsonFormatter("{0}")]
        public string BeforeParse { get; set; }

        /// <summary>
        /// A two-dimensional array representing the input data on tabular form. This input can be used when the data is already parsed, for example from a grid view component. Each cell can be a string or number. If not switchRowsAndColumns is set, the columns are interpreted as series.
        /// Default: undefined
        /// </summary>
        public object[,] Columns { get; set; }

        /// <summary>
        /// A URL to a remote JSON dataset, structured as a column array. Will be fetched when the chart is created using Ajax.
        /// Default: undefined
        /// </summary>
        public string ColumnsURL { get; set; }

        /// <summary>
        /// The callback that is evaluated when the data is finished loading, optionally from an external source, and parsed. The first argument passed is a finished chart options object, containing the series. These options can be extended with additional options and passed directly to the chart constructor.
        /// Default: undefined
        /// </summary>
        [JsonFormatter("{0)")]
        public string Complete { get; set; }

        /// <summary>
        /// A comma delimited string to be parsed. Related options are startRow, endRow, startColumn and endColumn to delimit what part of the table is used. The lineDelimiter and itemDelimiter options define the CSV delimiter formats.
        /// The built-in CSV parser doesn't support all flavours of CSV, so in some cases it may be necessary to use an external CSV parser. See this example of parsing CSV through the MIT licensed Papa Parse library.
        /// Default: undefined
        /// </summary>
        public string Csv { get; set; }

        /// <summary>
        /// A URL to a remote CSV dataset. Will be fetched when the chart is created using Ajax.
        /// Default: undefined
        /// </summary>
        public string CsvURL { get; set; }

        /// <summary>
        /// Sets the refresh rate for data polling when importing remote dataset by setting data.csvURL, data.rowsURL, data.columnsURL, or data.googleSpreadsheetKey.
        /// Note that polling must be enabled by setting data.enablePolling to true.
        /// The value is the number of seconds between pollings. It cannot be set to less than 1 second.
        /// Default: 1
        /// </summary>
        public Number? DataRefreshRate { get; set; }

        /// <summary>
        /// Which of the predefined date formats in Date.prototype.dateFormats to use to parse date values. Defaults to a best guess based on what format gives valid and ordered dates.
        /// Valid options include:
        /// YYYY/mm/dd
        /// dd/mm/YYYY
        /// mm/dd/YYYY
        /// dd/mm/YY
        /// mm/dd/YY
        /// Default: undefined
        /// </summary>
        public string DateFormat { get; set; }

        /// <summary>
        /// The decimal point used for parsing numbers in the CSV
        /// If both this and data.delimiter is set to false, the parser will attempt to deduce the decimal point automatically.
        /// Default: . .
        /// </summary>
        public string DecimalPoint { get; set; }

        /// <summary>
        /// Enables automatic refetching of remote datasets every n seconds (defined by setting data.dataRefreshRate).
        /// Only works when either data.csvURL, data.rowsURL, data.columnsURL, or data.googleSpreadsheetKey.
        /// Default: false
        /// </summary>
        public bool? EnablePolling { get; set; }

        /// <summary>
        /// In tabular input data, the last column (indexed by 0) to use. Defaults to the last column containing data.
        /// Default: undefined
        /// </summary>
        public Number? EndColumn { get; set; }

        /// <summary>
        /// In tabular input data, the last row (indexed by 0) to use. Defaults to the last row containing data.
        /// Default: undefined
        /// </summary>
        public Number? EndRow { get; set; }

        /// <summary>
        /// Whether to use the first row in the data set as series names.
        /// Default: true
        /// </summary>
        public bool? FirstRowAsNames { get; set; }

        /// <summary>
        /// The key for a Google Spreadsheet to load. See general information on GS.
        /// Default: undefined
        /// </summary>
        public string GoogleSpreadsheetKey { get; set; }

        /// <summary>
        /// The Google Spreadsheet worksheet to use in combination with googleSpreadsheetKey. The available id's from your sheet can be read from https://spreadsheets.google.com/feeds/worksheets/{key}/public/basic.
        /// Default: undefined
        /// </summary>
        public string GoogleSpreadsheetWorksheet { get; set; }

        /// <summary>
        /// Item or cell delimiter for parsing CSV. Defaults to the tab character \t if a tab character is found in the CSV string, if not it defaults to ,.
        /// If this is set to false or undefined, the parser will attempt to deduce the delimiter automatically.
        /// Default: undefined
        /// </summary>
        public string ItemDelimiter { get; set; }

        /// <summary>
        /// Line delimiter for parsing CSV.
        /// Default: \n
        /// </summary>
        public string LineDelimiter { get; set; }

        /// <summary>
        /// A callback function to access the parsed columns, the two-dimentional input data array directly, before they are interpreted into series data and categories. Return false to stop completion, or call this.complete() to continue async.
        /// Default: undefined
        /// </summary>
        [JsonFormatter("{0)")]
        public string Parsed { get; set; }

        /// <summary>
        /// A callback function to parse string representations of dates into JavaScript timestamps. Should return an integer timestamp on success.
        /// Default: undefined
        /// </summary>
        [JsonFormatter("{0)")]
        public string ParseDate { get; set; }

        /// <summary>
        /// The same as the columns input option, but defining rows intead of columns.
        /// Default: undefined
        /// </summary>
        public object[,] Rows { get; set; }

        /// <summary>
        /// A URL to a remote JSON dataset, structured as a row array. Will be fetched when the chart is created using Ajax.
        /// Default: undefined
        /// </summary>
        public string RowsURL { get; set; }

        /// <summary>
        /// An array containing object with Point property names along with what column id the property should be taken from.
        /// Default: undefined
        /// </summary>
        public object[] SeriesMapping { get; set; }

        /// <summary>
        /// In tabular input data, the first column (indexed by 0) to use.
        /// Default: 0
        /// </summary>
        public Number? StartColumn { get; set; }

        /// <summary>
        /// In tabular input data, the first row (indexed by 0) to use.
        /// Default: 0
        /// </summary>
        public Number? StartRow { get; set; }

        /// <summary>
        /// Switch rows and columns of the input data, so that this.columns effectively becomes the rows of the data set, and the rows are interpreted as series.
        /// Default: false
        /// </summary>
        public bool? SwitchRowsAndColumns { get; set; }

        /// <summary>
        /// An HTML table or the id of such to be parsed as input data. Related options are startRow, endRow, startColumn and endColumn to delimit what part of the table is used.
        /// Default: undefined
        /// </summary>
        public string Table { get; set; }
    }
}
