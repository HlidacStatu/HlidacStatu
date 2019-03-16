using DotNet.Highcharts.Enums;

namespace DotNet.Highcharts.Options.ParallelAxes
{
    /// <summary>
    /// Titles for yAxes are taken from xAxis.categories. All options for xAxis.labels applies to parallel coordinates titles. For example, to style categories, use xAxis.labels.style.
    /// </summary>
    public class ParallelAxesTitle
    {
        /// <summary>
        /// Alignment of the text, can be "left", "right" or "center". Default alignment depends on the title.align:
        /// Default: undefined
        /// </summary>
        public HorizontalAligns TextAlign { get; set; }
    }
}
