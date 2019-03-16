using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotNet.Highcharts.Helpers;

namespace DotNet.Highcharts.Options.Boost
{
    /// <summary>
    /// Options for the Boost module. The Boost module allows certain series types to be rendered by WebGL instead of the default SVG. This allows hundreds of thousands of data points to be rendered in milliseconds. In addition to the WebGL rendering it saves time by skipping processing and inspection of the data wherever possible. This introduces some limitations to what features are available in Boost mode. See the docs for details.
    /// In addition to the global boost option, each series has a boostThreshold that defines when the boost should kick in.
    /// Requires the modules/boost.js module.
    /// </summary>
    public class Boost
    {

        /// <summary>
        /// If set to true, the whole chart will be boosted if one of the series crosses its threshold, and all the series can be boosted.
        /// Default: true
        /// </summary>
        public bool? AllowForce { get; set; }

        /// <summary>
        /// Debugging options for boost. Useful for benchmarking, and general timing.
        /// </summary>
        public BoostDebug Debug { get; set; }

        /// <summary>
        /// Enable or disable boost on a chart.
        /// Default: true
        /// </summary>
        public bool? Enabled { get; set; }

        /// <summary>
        /// Set the series threshold for when the boost should kick in globally.
        /// Setting to e.g. 20 will cause the whole chart to enter boost mode if there are 20 or more series active. When the chart is in boost mode, every series in it will be rendered to a common canvas. This offers a significant speed improvment in charts with a very high amount of series.
        /// Default: null
        /// </summary>
        public Number? SeriesThreshold { get; set; }

        /// <summary>
        /// Enable or disable GPU translations. GPU translations are faster than doing the translation in JavaScript.
        /// This option may cause rendering issues with certain datasets. Namely, if your dataset has large numbers with small increments (such as timestamps), it won't work correctly. This is due to floating point precission.
        /// Default: false
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public bool? UseGPUTranslations { get; set; }
    }
}
