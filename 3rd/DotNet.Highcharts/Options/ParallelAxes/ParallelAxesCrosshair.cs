using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using DotNet.Highcharts.Enums;
using DotNet.Highcharts.Helpers;

namespace DotNet.Highcharts.Options.ParallelAxes
{
    /// <summary>
    /// Configure a crosshair that follows either the mouse pointer or the hovered point.
    /// In styled mode, the crosshairs are styled in the .highcharts-crosshair, .highcharts-crosshair-thin or .highcharts-xaxis-category classes.
    /// </summary>
    public class ParallelAxesCrosshair
    {
        /// <summary>
        /// A class name for the crosshair, especially as a hook for styling.
        /// Default: undefined
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// The color of the crosshair. Defaults to #cccccc for numeric and datetime axes, and rgba(204,214,235,0.25) for category axes, where the crosshair by default highlights the whole category.
        /// Default: #CCCCCC
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// The dash style for the crosshair. See series.dashStyle for possible values.
        /// Default: Solid
        /// </summary>
        public DashStyles DashStyle { get; set; }

        /// <summary>
        /// Whether the crosshair should snap to the point or follow the pointer independent of points.
        /// Default: true
        /// </summary>
        public bool? Snap { get; set; }

        /// <summary>
        /// The pixel width of the crosshair. Defaults to 1 for numeric or datetime axes, and for one category width for category axes.
        /// Default: 1
        /// </summary>
        /// 
        public Number? Width { get; set; }
        /// <summary>
        /// The Z index of the crosshair. Higher Z indices allow drawing the crosshair on top of the series or behind the grid lines.
        /// Default: 2
        /// </summary>
        public Number? ZIndex { get; set; }
    }
}
