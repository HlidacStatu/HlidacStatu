using DotNet.Highcharts.Helpers;
// ReSharper disable InconsistentNaming

namespace DotNet.Highcharts.Options.Options3d
{
    /// <summary>
    /// Options to render charts in 3 dimensions. This feature requires highcharts-3d.js, found in the download package or online at code.highcharts.com/highcharts-3d.js.
    /// </summary>
    public class Options3d
    {
        /// <summary>
        /// One of the two rotation angles for the chart.
        /// Default: 0
        /// </summary>
        public Number? Alpha { get; set; }

        /// <summary>
        /// Set it to "auto" to automatically move the labels to the best edge.
        /// Default: null
        /// </summary>
        public string AxisLabelPosition { get; set; }

        /// <summary>
        /// One of the two rotation angles for the chart.
        /// Default: 0
        /// </summary>
        public Number? Beta { get; set; }

        /// <summary>
        /// The total depth of the chart.
        /// Default: 100
        /// </summary>
        public Number? Depth { get; set; }

        /// <summary>
        /// Wether to render the chart using the 3D functionality.
        /// Default: true
        /// </summary>
        public bool? Enabled { get; set; }

        /// <summary>
        /// Whether the 3d box should automatically adjust to the chart plot area.
        /// Default: true
        /// </summary>
        public bool? FitToPlot { get; set; }

        /// <summary>
        /// Provides the option to draw a frame around the charts by defining a bottom, front and back panel.
        /// </summary>
        public Options3dFrame Frame {get;set;}

        /// <summary>
        /// Defines the distance the viewer is standing in front of the chart, this setting is important to calculate the perspective effect in column and scatter charts. It is not used for 3D pie charts.
        /// Default: 100
        /// </summary>
        public Number? ViewDistance { get; set; }
    }
}
