using DotNet.Highcharts.Helpers;
using DotNet.Highcharts.Options;
using DotNet.Highcharts.Options.Accessibility;
using DotNet.Highcharts.Options.Exporting;
using DotNet.Highcharts.Options.Legend;
using DotNet.Highcharts.Options.PlotOptions;
using DotNet.Highcharts.Options.Series;
using DotNet.Highcharts.Options.XAxis;
using DotNet.Highcharts.Options.YAxis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using DotNet.Highcharts.Options.Boost;
using DotNet.Highcharts.Options.ColorAxis;
using DotNet.Highcharts.Options.DataOptions;

namespace DotNet.Highcharts
{
    public class Highcharts : IHtmlString
    {
        internal string Name { get; }
        internal string ContainerName { get; }

        internal bool UseJQueryPlugin { get; }

        internal IDictionary<string, string> JsVariables { get; }
        internal IDictionary<string, string> JsFunctions { get; }

        internal GlobalOptions Options { get; private set; }

        internal string FunctionName { get; private set; }

        internal ContainerOptions ContainerOptions { get; private set; }

        private Accessibility _accessibility;
        private Boost _boost;
        private Chart _chart;
        private ColorAxis _colorAxis;
        private DataOptions _dataOptions;
        private Credits _credits;
        private Labels _labels;
        private Legend _legend;
        private Loading _loading;
        private PlotOptions _plotOptions;
        private Pane _pane;
        private Pane[] _paneArray;
        private Series _series;
        private Series[] _seriesArray;
        private Subtitle _subtitle;
        private Title _title;
        private Tooltip _tooltip;
        private XAxis _xAxis;
        private XAxis[] _xAxisArray;
        private YAxis _yAxis;
        private YAxis[] _yAxisArray;
        private Exporting _exporting;
        private Navigation _navigation;

        /// <summary>
        /// The chart object is the JavaScript object representing a single chart in the web page.
        /// </summary>
        /// <param name="name">The object name of the chart and related container</param>
        /// <param name="useJQueryPlugin"></param>
        /// <see cref="http://www.highcharts.com/ref/"/>
        public Highcharts(string name, bool useJQueryPlugin = false)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("The name of the chart must be specified.");

            Name = name;
            UseJQueryPlugin = useJQueryPlugin;
            ContainerName = "{0}_container".FormatWith(name);
            JsVariables = new Dictionary<string, string>();
            JsFunctions = new Dictionary<string, string>();
        }

        /// <summary>
        /// Global options that don't apply to each chart. These options, like the lang options, must be set using the Highcharts.setOptions method.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public Highcharts SetOptions(GlobalOptions options)
        {
            Options = options;
            return this;
        }

        /// <summary>
        /// Options regarding the chart area and plot area as well as general chart options.
        /// </summary>
        /// <param name="chart"></param>
        /// <returns></returns>
        public Highcharts InitChart(Chart chart)
        {
            _chart = chart;
            return this;
        }

        /// <summary>
        /// Options regarding the chart accessibility options.
        /// </summary>
        /// <param name="accessibility"></param>
        /// <returns></returns>
        public Highcharts SetAccessibility(Accessibility accessibility)
        {
            _accessibility = accessibility;
            return this;
        }

        /// <summary>
        /// Options regarding boost. Using boost enables WebGL rendering instead of the default Javascript rendering.
        /// </summary>
        /// <param name="boost"></param>
        /// <returns></returns>
        public Highcharts SetBoost(Boost boost)
        {
            _boost = boost;
            return this;
        }

        /// <summary>
        /// Highchart by default puts a credits label in the lower right corner of the chart. 
        /// This can be changed using these options.
        /// </summary>
        public Highcharts SetCredits(Credits credits)
        {
            _credits = credits;
            return this;
        }

        /// <summary>
        /// Sets the color axis settings.
        /// </summary>
        /// <param name="colorAxis"></param>
        /// <returns></returns>
        public Highcharts ColorAxis(ColorAxis colorAxis)
        {
            _colorAxis = colorAxis;
            return this;
        }

        /// <summary>
        /// Sets the Highcharts data option.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public Highcharts SetDataOptions(DataOptions data)
        {
            _dataOptions = data;
            return this;
        }

        /// <summary>
        /// HTML labels that can be positioined anywhere in the chart area.
        /// </summary>
        /// <param name="labels"></param>
        /// <returns></returns>
        public Highcharts SetLabels(Labels labels)
        {
            _labels = labels;
            return this;
        }

        /// <summary>
        /// The legend is a box containing a symbol and name for each series item or point item in the chart.
        /// </summary>
        /// <param name="legend"></param>
        /// <returns></returns>
        public Highcharts SetLegend(Legend legend)
        {
            _legend = legend;
            return this;
        }

        /// <summary>
        /// The loading options control the appearance of the loading screen that covers the plot area on chart operations. 
        /// This screen only appears after an explicit call to chart.showLoading(). It is a utility for developers to 
        /// communicate to the end user that something is going on, for example while retrieving new data via an XHR connection. 
        /// The "Loading..." text itself is not part of this configuration object, but part of the lang object.
        /// </summary>
        /// <param name="loading"></param>
        /// <returns></returns>
        public Highcharts SetLoading(Loading loading)
        {
            _loading = loading;
            return this;
        }

        /// <summary>
        /// The plotOptions is a wrapper object for config objects for each series type. The config objects for each series 
        /// can also be overridden for each series item as given in the series array.
        /// Configuration options for the series are given in three levels. Options for all series in a chart are given in 
        /// the plotOptions.series object. Then options for all series of a specific type are given in the plotOptions of 
        /// that type, for example plotOptions.line. Next, options for one single series are given in the series array.
        /// </summary>
        /// <param name="plotOptions"></param>
        /// <returns></returns>
        public Highcharts SetPlotOptions(PlotOptions plotOptions)
        {
            _plotOptions = plotOptions;
            return this;
        }

        /// <summary>
        /// Applies only to polar charts and angular gauges. This configuration object holds general options for the combined X and Y axes set. Each xAxis or yAxis can reference the pane by index.
        /// </summary>
        /// <param name="paneArray"></param>
        /// <returns></returns>
        public Highcharts SetPane(Pane[] paneArray)
        {
            _paneArray = paneArray;
            return this;
        }

        public Highcharts SetPane(Pane pane)
        {
            _pane = pane;
            return this;
        }

        /// <summary>
        /// The actual series to append to the chart. In addition to the members listed below, any member of the plotOptions 
        /// for that specific type of plot can be added to a series individually. For example, even though a general lineWidth 
        /// is specified in plotOptions.series, an individual lineWidth can be specified for each series.
        /// </summary>
        /// <param name="series"></param>
        /// <returns></returns>
        public Highcharts SetSeries(Series series)
        {
            _series = series;
            return this;
        }
        public Highcharts SetSeries(Series[] seriesArray)
        {
            _seriesArray = seriesArray;
            return this;
        }

        /// <summary>
        /// The chart's subtitle
        /// </summary>
        /// <param name="subtitle"></param>
        /// <returns></returns>
        public Highcharts SetSubtitle(Subtitle subtitle)
        {
            _subtitle = subtitle;
            return this;
        }

        /// <summary>
        /// The chart's main title.
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public Highcharts SetTitle(Title title)
        {
            _title = title;
            return this;
        }
        public Title GetTitle()
        {
            return this._title;
        }

        /// <summary>
        /// Options for the tooltip that appears when the user hovers over a series or point.
        /// </summary>
        /// <param name="tooltip"></param>
        /// <returns></returns>
        public Highcharts SetTooltip(Tooltip tooltip)
        {
            _tooltip = tooltip;
            return this;
        }

        /// <summary>
        /// The X axis or category axis. Normally this is the horizontal axis, though if the chart is inverted this is the vertical axis. 
        /// In case of multiple axes, the xAxis node is an array of configuration objects.
        /// </summary>
        /// <param name="xAxis"></param>
        /// <returns></returns>
        public Highcharts SetXAxis(XAxis xAxis)
        {
            _xAxis = xAxis;
            return this;
        }
        public Highcharts SetXAxis(XAxis[] xAxisArray)
        {
            _xAxisArray = xAxisArray;
            return this;
        }

        /// <summary>
        /// The Y axis or value axis. Normally this is the vertical axis, though if the chart is inverted this is the horiontal axis. 
        /// In case of multiple axes, the yAxis node is an array of configuration objects.
        /// </summary>
        /// <param name="yAxis"></param>
        /// <returns></returns>
        public Highcharts SetYAxis(YAxis yAxis)
        {
            _yAxis = yAxis;
            return this;
        }
        public Highcharts SetYAxis(YAxis[] yAxisArray)
        {
            _yAxisArray = yAxisArray;
            return this;
        }

        /// <summary>
        /// Options for the exporting module.
        /// </summary>
        /// <param name="exporting"></param>
        /// <returns></returns>
        public Highcharts SetExporting(Exporting exporting)
        {
            _exporting = exporting;
            return this;
        }

        /// <summary>
        /// A collection of options for buttons and menus appearing in the exporting module.
        /// </summary>
        /// <param name="navigation"></param>
        /// <returns></returns>
        public Highcharts SetNavigation(Navigation navigation)
        {
            _navigation = navigation;
            return this;
        }

        public Highcharts SetContainerOptions(ContainerOptions options)
        {
            ContainerOptions = options;
            return this;
        }

        /// <summary>
        /// Add the javascript variable to the same jQuery document ready where chart is initialized.
        /// Variables are added before the chart.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="value">The value of the variable.</param>
        /// <returns></returns>
        public Highcharts AddJavascripVariable(string name, string value)
        {
            JsVariables.Add(name, value);
            return this;
        }

        /// <summary>
        /// Add javascript function to the same jQuery document ready where chart is initialized.
        /// The functions are added after the chart.
        /// </summary>
        /// <param name="name">The name of the function.</param>
        /// <param name="body">The body of the function.</param>
        /// <param name="variables">The variables of the function.</param>
        /// <returns></returns>
        public Highcharts AddJavascripFunction(string name, string body, params string[] variables)
        {
            JsFunctions.Add("function {0}({1}){{".FormatWith(name, string.Join(", ", variables)), body);
            return this;
        }

        public Highcharts InFunction(string name)
        {
            FunctionName = name;
            return this;
        }

        #region IHtmlString Members

        public virtual string ToHtmlString()
        {
            StringBuilder scripts = new StringBuilder();

            string style = "";
            if (ContainerOptions != null && ContainerOptions.MatchParentHeight)
                style = "style='height: 100%;'";

            scripts.AppendLine("<div id='{0}' {1}></div>".FormatWith(ContainerName, style));
            scripts.AppendLine("<script type='text/javascript'>");
            if (Options != null)
                scripts.AppendLine("Highcharts.setOptions({0});".FormatWith(JsonSerializer.Serialize(Options)));

            scripts.AppendLine("var {0};".FormatWith(Name));
            scripts.AppendLine(!string.IsNullOrEmpty(FunctionName) ? $"function {FunctionName}() {{" : "$(document).ready(function() {");
            scripts.AppendHighchart(this);
            scripts.AppendLine(!string.IsNullOrEmpty(FunctionName) ? "}" : "});");

            scripts.AppendLine("</script>");

            return scripts.ToString();
        }

        #endregion

        public virtual string GetOptions()
        {
            StringBuilder options = new StringBuilder();

            if (UseJQueryPlugin)
            {
                if (_chart != null)
                {
                    options.Append("chart: {{ {0} }}".FormatWith(JsonSerializer.Serialize(_chart, false)));
                }
            }
            else
            {
                options.Append(_chart != null ? 
                    "chart: {{ renderTo:'{0}', {1} }}".FormatWith(ContainerName, JsonSerializer.Serialize(_chart, false)) : 
                    "chart: {{ renderTo:'{0}' }}".FormatWith(ContainerName));
            }

            if (_accessibility != null)
            {
                options.AppendLine(", ");
                options.Append("accessibility: {0}".FormatWith(JsonSerializer.Serialize(_accessibility), 2));
            }

            if (_boost != null)
            {
                options.AppendLine(", ");
                options.Append("boost: {0}".FormatWith(JsonSerializer.Serialize(_boost)), 2);
            }

            if (_credits != null)
            {
                options.AppendLine(", ");
                options.Append("credits: {0}".FormatWith(JsonSerializer.Serialize(_credits)), 2);
            }

            if (_colorAxis != null)
            {
                options.AppendLine(", ");
                options.Append("colorAxis: {0}".FormatWith(JsonSerializer.Serialize(_colorAxis)), 2);
            }

            if (_dataOptions != null)
            {
                options.AppendLine(", ");
                options.AppendLine("data: {0}".FormatWith(JsonSerializer.Serialize(_dataOptions)), 2);
            }

            if (_labels != null)
            {
                options.AppendLine(", ");
                options.Append("labels: {0}".FormatWith(JsonSerializer.Serialize(_labels)), 2);
            }

            if (_legend != null)
            {
                options.AppendLine(", ");
                options.Append("legend: {0}".FormatWith(JsonSerializer.Serialize(_legend)), 2);
            }

            if (_loading != null)
            {
                options.AppendLine(", ");
                options.Append("loading: {0}".FormatWith(JsonSerializer.Serialize(_loading)), 2);
            }

            if (_plotOptions != null)
            {
                options.AppendLine(", ");
                options.Append("plotOptions: {0}".FormatWith(JsonSerializer.Serialize(_plotOptions)), 2);
            }

            if (_pane != null)
            {
                options.AppendLine(", ");
                options.Append("pane: {0}".FormatWith(JsonSerializer.Serialize(_pane)), 2);
            }

            if (_paneArray != null)
            {
                options.AppendLine(", ");
                options.Append("pene: {0}".FormatWith(JsonSerializer.Serialize(_paneArray)), 2);
            }

            if (_subtitle != null)
            {
                options.AppendLine(", ");
                options.Append("subtitle: {0}".FormatWith(JsonSerializer.Serialize(_subtitle)), 2);
            }

            if (_title?.Text == null)
            {
                options.AppendLine(", ");
                options.Append("title: { text : undefined }");
            }
            else //_title != null
            {
                options.AppendLine(", ");
                options.Append("title: {0}".FormatWith(JsonSerializer.Serialize(_title)), 2);
            }


            if (_tooltip != null)
            {
                options.AppendLine(", ");
                options.Append("tooltip: {0}".FormatWith(JsonSerializer.Serialize(_tooltip)), 2);
            }

            if (_xAxis != null)
            {
                options.AppendLine(", ");
                options.Append("xAxis: {0}".FormatWith(JsonSerializer.Serialize(_xAxis)), 2);
            }

            if (_xAxisArray != null)
            {
                options.AppendLine(", ");
                options.Append("xAxis: {0}".FormatWith(JsonSerializer.Serialize(_xAxisArray)), 2);
            }

            if (_yAxis != null)
            {
                options.AppendLine(", ");
                options.Append("yAxis: {0}".FormatWith(JsonSerializer.Serialize(_yAxis)), 2);
            }
            else if (_yAxisArray != null)
            {
                options.AppendLine(", ");
                options.Append("yAxis: {0}".FormatWith(JsonSerializer.Serialize(_yAxisArray)), 2);
            }

            if (_exporting != null)
            {
                options.AppendLine(", ");
                options.Append("exporting: {0}".FormatWith(JsonSerializer.Serialize(_exporting)), 2);
            }

            if (_navigation != null)
            {
                options.AppendLine(", ");
                options.Append("navigation: {0}".FormatWith(JsonSerializer.Serialize(_navigation)), 2);
            }

            if (_series != null)
            {
                options.AppendLine(", ");
                options.Append("series: [{0}]".FormatWith(JsonSerializer.Serialize(_series)), 2);
            }
            else if (_seriesArray != null)
            {
                options.AppendLine(", ");
                options.Append("series: {0}".FormatWith(JsonSerializer.Serialize(_seriesArray)), 2);
            }
            options.AppendLine();

            return options.ToString();
        }
    }
}