using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using DotNet.Highcharts.Enums;

namespace DotNet.Highcharts
{
    public static class Extensions
    {
        public static string FormatWith(this string format, params object[] parameters) { return string.Format(format, parameters); }

        public static string FormatWith(this string format, int? value)
        {
            if (value != null)
                return string.Format(format, value.Value.ToString(CultureInfo.InvariantCulture));
            return string.Empty;
        }

        public static string FormatWith(this string format, ChartTypes? value)
        {
            if (value != null)
                return string.Format(format, value.Value.ToString().ToLowerInvariant());
            return string.Empty;
        }

        public static void Append(this StringBuilder sb, string value, int tabs)
        {
            string tabsInBuilder = String.Empty;
            for (int i = 0; i < tabs; i++)
                tabsInBuilder += "\t";

            sb.Append(tabsInBuilder + value);
        }

        public static void AppendLine(this StringBuilder sb, string value, int tabs)
        {
            string tabsInBuilder = String.Empty;
            for (int i = 0; i < tabs; i++)
                tabsInBuilder += "\t";

            sb.AppendLine(tabsInBuilder + value);
        }

        public static void AppendWidthCommaBefore(this StringBuilder sb, string value)
        {
            if (sb.Length > 0)
                sb.Append(", ");
            sb.Append(value);
        }

        public static void AppendWidthCommaBefore(this StringBuilder sb, int? value)
        {
            if (sb.Length > 0)
                sb.Append(", ");
            if (value != null)
                sb.Append(value.Value.ToString(CultureInfo.InvariantCulture));
        }

        public static void AppendHighchart(this StringBuilder sb, Highcharts chart)
        {
            foreach (KeyValuePair<string, string> jsVariable in chart.JsVariables)
                sb.AppendLine("var {0} = {1};".FormatWith(jsVariable.Key, jsVariable.Value), 1);

            if (chart.UseJQueryPlugin)
            {
                sb.AppendLine("$('" + chart.ContainerName + "'.highcharts({", 1);
            }
            else
            {
                sb.AppendLine(chart.Name + " = new Highcharts.Chart({", 1);
            }

            sb.Append(chart.GetOptions(), 2);
            sb.AppendLine("});", 1);

            foreach (KeyValuePair<string, string> jsFunction in chart.JsFunctions)
            {
                sb.AppendLine();
                sb.AppendLine(jsFunction.Key, 1);
                sb.AppendLine(jsFunction.Value, 2);
                sb.AppendLine("}", 1);
            }
        }

        public static void AppendStockchart(this StringBuilder sb, Highstock chart)
        {
            foreach (KeyValuePair<string, string> jsVariable in chart.JsVariables)
                sb.AppendLine("var {0} = {1};".FormatWith(jsVariable.Key, jsVariable.Value), 1);

            if (chart.UseJQueryPlugin)
            {
                sb.AppendLine("$('" + chart.ContainerName + "'.highcharts('StockChart', {", 1);
            }
            else
            {
                sb.AppendLine(chart.Name + " = new Highcharts.StockChart({", 1);
            }

            sb.Append(chart.GetOptions(), 2);
            sb.AppendLine("});", 1);

            foreach (KeyValuePair<string, string> jsFunction in chart.JsFunctions)
            {
                sb.AppendLine();
                sb.AppendLine(jsFunction.Key, 1);
                sb.AppendLine(jsFunction.Value, 2);
                sb.AppendLine("}", 1);
            }
        }
    }
}