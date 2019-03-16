using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace DotNet.Highcharts
{
    public class Container : IHtmlString
    {
        readonly List<Highcharts> _Highcharts;

        public Container(IEnumerable<Highcharts> highcharts)
        {
            _Highcharts = new List<Highcharts>();
            _Highcharts.AddRange(highcharts);
        }

        #region IHtmlString Members

        public string ToHtmlString()
        {
            StringBuilder scripts = new StringBuilder();
            _Highcharts.ForEach(x =>
            {
                string style = "";
                if (x.ContainerOptions != null && x.ContainerOptions.MatchParentHeight)
                    style = "style='height: 100%;'";

                scripts.AppendLine("<div id='{0}' {1}></div>".FormatWith(x.ContainerName, style));
            });

            List<Highcharts> startupCharts = _Highcharts.Where(x => string.IsNullOrEmpty(x.FunctionName)).ToList();
            scripts.AppendLine("<script type='text/javascript'>");
            startupCharts.ForEach(x => scripts.AppendLine("var {0};".FormatWith(x.Name)));
            scripts.AppendLine("$(document).ready(function() {");
            startupCharts.ForEach(scripts.AppendHighchart);
            scripts.AppendLine("});");

            List<Highcharts> functionCharts = _Highcharts.Where(x => !string.IsNullOrEmpty(x.FunctionName)).ToList();
            foreach (Highcharts chart in functionCharts)
            {
                scripts.AppendLine("var {0};".FormatWith(chart.Name));
                scripts.AppendLine(string.Format("function {0}() {{", chart.FunctionName));
                scripts.AppendHighchart(chart);
                scripts.AppendLine("}");
            }
            scripts.AppendLine("</script>");

            return scripts.ToString();
        }

        #endregion
    }
}