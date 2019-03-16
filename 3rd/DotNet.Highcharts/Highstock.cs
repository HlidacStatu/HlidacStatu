using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using DotNet.Highcharts.Helpers;
using DotNet.Highcharts.Options;
using DotNet.Highcharts.Options.Exporting;
using DotNet.Highcharts.Options.Legend;
using DotNet.Highcharts.Options.PlotOptions;
using DotNet.Highcharts.Options.Series;
using DotNet.Highcharts.Options.XAxis;
using DotNet.Highcharts.Options.YAxis;

namespace DotNet.Highcharts
{
    public class Highstock : Highcharts
    {

        RangeSelector _RangeSelector;
        Navigator _Navigator;
        Scrollbar _Scrollbar;

        /// <summary>
        /// The chart object is the JavaScript object representing a single chart in the web page.
        /// </summary>
        /// <param name="name">The object name of the chart and related container</param>
        /// <see cref="http://www.highcharts.com/ref/"/>
        public Highstock(string name, bool useJQueryPlugin = false)
            : base(name, useJQueryPlugin)
        {
        }

        /// <summary>
        /// Global options that don't apply to each chart. These options, like the lang options, must be set using the Highstock.setOptions method.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public new Highstock SetOptions(GlobalOptions options)
        {
            base.SetOptions(options);
            return this;
        }

        /// <summary>
        /// Options regarding the chart area and plot area as well as general chart options.
        /// </summary>
        /// <param name="chart"></param>
        /// <returns></returns>
        public new Highstock InitChart(Chart chart)
        {
            base.InitChart(chart);
            return this;
        }

        /// <summary>
        /// Highchart by default puts a credits label in the lower right corner of the chart. 
        /// This can be changed using these options.
        /// </summary>
        public new Highstock SetCredits(Credits credits)
        {
            base.SetCredits(credits);
            return this;
        }

        /// <summary>
        /// HTML labels that can be positioined anywhere in the chart area.
        /// </summary>
        /// <param name="labels"></param>
        /// <returns></returns>
        public new Highstock SetLabels(Labels labels)
        {
            base.SetLabels(labels);
            return this;
        }

        /// <summary>
        /// The legend is a box containing a symbol and name for each series item or point item in the chart.
        /// </summary>
        /// <param name="legend"></param>
        /// <returns></returns>
        public new Highstock SetLegend(Legend legend)
        {
            base.SetLegend(legend);
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
        public new Highstock SetLoading(Loading loading)
        {
            base.SetLoading(loading);
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
        public new Highstock SetPlotOptions(PlotOptions plotOptions)
        {
            base.SetPlotOptions(plotOptions);
            return this;
        }

        /// <summary>
        /// The range selector is a tool for selecting ranges to display within the chart.
        /// It provides buttons to select preconfigured ranges in the chart,
        /// like 1 day, 1 week, 1 month etc. It also provides input boxes
        /// where min and max dates can be manually input.
        /// This object contains all the options which can be applied to this part of the chart.
        /// </summary>
        /// <remarks>This is available only with Highstock.</remarks>
        /// <param name="rangeSelector">
        /// A reference to a <see cref="RangeSelector"/> object containing the selected options.
        /// </param>
        /// <returns>A reference to this class, useful for chained settings.</returns>
        public Highstock SetRangeSelector(RangeSelector rangeSelector)
        {
            _RangeSelector = rangeSelector;
            return this;
        }

        /// <summary>
        /// The actual series to append to the chart. In addition to the members listed below, any member of the plotOptions 
        /// for that specific type of plot can be added to a series individually. For example, even though a general lineWidth 
        /// is specified in plotOptions.series, an individual lineWidth can be specified for each series.
        /// </summary>
        /// <param name="series"></param>
        /// <returns></returns>
        public new Highstock SetSeries(Series series)
        {
            base.SetSeries(series);
            return this;
        }
        public new Highstock SetSeries(Series[] seriesArray)
        {
            base.SetSeries(seriesArray);
            return this;
        }

        /// <summary>
        /// The chart's subtitle
        /// </summary>
        /// <param name="subtitle"></param>
        /// <returns></returns>
        public new Highstock SetSubtitle(Subtitle subtitle)
        {
            base.SetSubtitle(subtitle);
            return this;
        }

        /// <summary>
        /// The chart's main title.
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public new Highstock SetTitle(Title title)
        {
            base.SetTitle(title);
            return this;
        }

        /// <summary>
        /// Options for the tooltip that appears when the user hovers over a series or point.
        /// </summary>
        /// <param name="tooltip"></param>
        /// <returns></returns>
        public new Highstock SetTooltip(Tooltip tooltip)
        {
            base.SetTooltip(tooltip);
            return this;
        }

        /// <summary>
        /// The X axis or category axis. Normally this is the horizontal axis, though if the chart is inverted this is the vertical axis. 
        /// In case of multiple axes, the xAxis node is an array of configuration objects.
        /// </summary>
        /// <param name="xAxis"></param>
        /// <returns></returns>
        public new Highstock SetXAxis(XAxis xAxis)
        {
            base.SetXAxis(xAxis);
            return this;
        }
        public new Highstock SetXAxis(XAxis[] xAxisArray)
        {
            base.SetXAxis(xAxisArray);
            return this;
        }

        /// <summary>
        /// The Y axis or value axis. Normally this is the vertical axis, though if the chart is inverted this is the horiontal axis. 
        /// In case of multiple axes, the yAxis node is an array of configuration objects.
        /// </summary>
        /// <param name="yAxis"></param>
        /// <returns></returns>
        public new Highstock SetYAxis(YAxis yAxis)
        {
            base.SetYAxis(yAxis);
            return this;
        }
        public new Highstock SetYAxis(YAxis[] yAxisArray)
        {
            base.SetYAxis(yAxisArray);
            return this;
        }

        /// <summary>
        /// Options for the exporting module.
        /// </summary>
        /// <param name="exporting"></param>
        /// <returns></returns>
        public new Highstock SetExporting(Exporting exporting)
        {
            base.SetExporting(exporting);
            return this;
        }

        /// <summary>
        /// A collection of options for buttons and menus appearing in the exporting module.
        /// </summary>
        /// <param name="navigation"></param>
        /// <returns></returns>
        public new Highstock SetNavigation(Navigation navigation)
        {
            base.SetNavigation(navigation);
            return this;
        }

        /// <summary>
        /// The navigator is a small series below the main series, displaying a view of the
        /// entire data set. It provides tools to zoom in and out on parts of the data
        /// as well as panning across the dataset.
        /// This object contains all the options which can be applied to this part of the chart.
        /// </summary>
        /// <remarks>This is available only with Highstock.</remarks>
        /// <param name="navigator">
        /// A reference to a <see cref="Navigator"/> object containing the selected options.
        /// </param>
        /// <returns>A reference to this class, useful for chained settings.</returns>
        public Highstock SetNavigator(Navigator navigator)
        {
            _Navigator = navigator;
            return this;
        }

        /// <summary>
        /// The scrollbar is a means of panning over the X axis of a chart.
        /// This object contains all the options which can be applied to this part of the chart.
        /// </summary>
        /// <remarks>This is available only with Highstock.</remarks>
        /// <param name="scrollbar">
        /// A reference to a <see cref="Scrollbar"/> object containing the selected options.
        /// </param>
        /// <returns>A reference to this class, useful for chained settings.</returns>
        public Highstock SetScrollbar(Scrollbar scrollbar)
        {
            _Scrollbar = scrollbar;
            return this;
        }

        /// <summary>
        /// Add the javascript variable to the same jQuery document ready where chart is initialized.
        /// Variables are added before the chart.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="value">The value of the variable.</param>
        /// <returns></returns>
        public new Highstock AddJavascripVariable(string name, string value)
        {
            base.AddJavascripVariable(name, value);
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
        public new Highstock AddJavascripFunction(string name, string body, params string[] variables)
        {
            base.AddJavascripFunction(name, body, variables);
            return this;
        }

        public new Highstock InFunction(string name)
        {
            base.InFunction(name);
            return this;
        }

        #region IHtmlString Members

        public override string ToHtmlString()
        {
            StringBuilder scripts = new StringBuilder();

            scripts.AppendLine("<div id='{0}'></div>".FormatWith(ContainerName));
            scripts.AppendLine("<script type='text/javascript'>");
            if (Options != null)
                scripts.AppendLine("Highcharts.setOptions({0});".FormatWith(JsonSerializer.Serialize(Options)));

            scripts.AppendLine("var {0};".FormatWith(Name));
            scripts.AppendLine(!string.IsNullOrEmpty(FunctionName) ? string.Format("function {0}() {{", FunctionName) : "$(document).ready(function() {");
            scripts.AppendStockchart(this);
            scripts.AppendLine(!string.IsNullOrEmpty(FunctionName) ? "}" : "});");
            
            scripts.AppendLine("</script>");

            return scripts.ToString();
        }

        #endregion

        public override string GetOptions()
        {
            StringBuilder options = new StringBuilder();
            options.Append(base.GetOptions());

            if (_Navigator != null)
            {
                options.AppendLine(", ");
                options.Append("navigator: {0}".FormatWith(JsonSerializer.Serialize(_Navigator)), 2);
            }

            if (_RangeSelector!= null)
            {
                options.AppendLine(", ");
                options.Append("rangeSelector: {0}".FormatWith(JsonSerializer.Serialize(_RangeSelector)), 2);
            }

            if (_Scrollbar != null)
            {
                options.AppendLine(", ");
                options.Append("scrollbar: {0}".FormatWith(JsonSerializer.Serialize(_Scrollbar)), 2);
            }

            options.AppendLine();

            return options.ToString();
        }
    }
}