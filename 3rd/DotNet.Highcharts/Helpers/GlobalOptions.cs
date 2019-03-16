using System.Drawing;
using DotNet.Highcharts.Attributes;
using DotNet.Highcharts.Options;

namespace DotNet.Highcharts.Helpers
{
    public class GlobalOptions
    {
        /// <summary>
        /// Global options that don't apply to each chart. These options, like the lang options, must be set using the Highcharts.setOptions method.
        /// </summary>
        /// <see cref="http://www.highcharts.com/ref/#global"/>
        public Global Global { get; set; }

        /// <summary>
        /// An array containing the default colors for the chart's series. When all colors are used, new colors are pulled from the start again. 
        /// Defaults to:
        /// <code>
        /// colors: ['#4572A7', '#AA4643', '#89A54E', '#80699B', '#3D96AE', '#DB843D', '#92A8CD', '#A47D7C', '#B5CA92']
        /// </code>
        /// </summary>
        /// <see cref="http://www.highcharts.com/ref/#colors"/>
        public Color[] Colors { get; set; }

        /// <summary>
        /// Language object. The language object is global and it can't be set on each chart initiation. 
        /// Instead, use Highcharts.setOptions to set it before any chart is initiated.
        /// </summary>
        /// <see cref="http://www.highcharts.com/ref/#lang"/>
        public Lang Lang { get; set; }
    }
}