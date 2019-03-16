﻿using DotNet.Highcharts.Attributes;
using DotNet.Highcharts.Enums;
using DotNet.Highcharts.Helpers;
using System.Drawing;

namespace DotNet.Highcharts.Options.ColorAxis
{
    /// <summary>
    /// A color axis for choropleth maps and heat maps. Visually, the color axis will 
    /// appear as a gradient or as separate items inside the legend, depending on whether 
    /// the axis is scalar or based on data classes.
    /// For supported color formats, see the docs article about colors.
    /// A scalar color axis is represented by a gradient.The colors either range between the
    /// minColor and the maxColor, or for more fine grained control the colors can be defined 
    /// in stops.Often times, the color axis needs to be adjusted to get the right color spread 
    /// for the data. In addition to stops, consider using a logarithmic axis type, or setting min 
    /// and max to avoid the colors being determined by outliers.
    /// When dataClasses are used, the ranges are subdivided into separate classes like categories 
    /// based on their values.This can be used for ranges between two values, but also for a true 
    /// category.However, when your data is categorized, it may be as convenient to add each category 
    /// to a separate series.
    /// See the Axis object for programmatic access to the axis.
    /// </summary>
    public class ColorAxis
    {
        /// <summary>
        /// When using multiple axis, the ticks of two or more opposite axes will automatically be aligned by adding ticks to the axis or axes with the least ticks, as if tickAmount were specified.
        /// This can be prevented by setting alignTicks to false. If the grid lines look messy, it's a good idea to hide them for the secondary axis by setting gridLineWidth to 0.
        /// If startOnTick or endOnTick in an Axis options are set to false, then the alignTicks will be disabled for the Axis.
        /// Disabled for logarithmic axes.
        /// Default: true
        /// </summary>
        public bool? AlignTicks { get; set; }

        /// <summary>
        /// The highest allowed value for automatically computed axis extremes.
        /// Default: undefined
        /// </summary>
        public Number? Ceiling { get; set; }

        /// <summary>
        /// A class name that opens for styling the axis by CSS, especially in Highcharts styled mode. The class name is applied to group elements for the grid, axis elements and labels.
        /// Default: undefined
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// Determines how to set each data class' color if no individual color is set. The default value, tween, computes intermediate colors between minColor and maxColor. The other possible value, category, pulls colors from the global or chart specific colors array.
        /// Default: tween
        /// </summary>
        /// // TODO: Make enum
        public string DataClassColor { get; set; }

        /// <summary>
        /// An array of data classes or ranges for the choropleth map. If none given, the color axis is scalar and values are distributed as a gradient between the minimum and maximum colors.
        /// </summary>
        public ColorAxisDataClass[] DataClasses { get; set; }

        /// <summary>
        /// Requires Accessibility module
        /// Description of the axis to screen reader users.
        /// Default: undefined
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Whether to force the axis to end on a tick. Use this option with the maxPadding option to control the axis end.
        /// Default: true
        /// </summary>
        public bool? EndOnTick { get; set; }

        /// <summary>
        /// Event handlers for the axis.
        /// </summary>
        public ColorAxisEvents Events { get; set; }

        /// <summary>
        /// The lowest allowed value for automatically computed axis extremes.
        /// Default: undefined
        /// </summary>
        public Number? Floor { get; set; }

        /// <summary>
        /// Color of the grid lines extending from the axis across the gradient.
        /// Default: #e6e6e6
        /// </summary>
        public Color? GridLineColor { get; set; }

        /// <summary>
        /// The dash or dot style of the grid lines. For possible values, see this demonstration.
        /// Default: Solid
        /// </summary>
        public DashStyles? GridLineDashStyle { get; set; }

        /// <summary>
        /// The width of the grid lines extending from the axis across the gradient of a scalar color axis.
        /// Default: 1
        /// </summary>
        public Number? GridLineWidth { get; set; }

        /// <summary>
        /// The Z index of the grid lines.
        /// Default: 1
        /// </summary>
        public Number? GridZIndex { get; set; }

        /// <summary>
        /// An id for the axis. This can be used after render time to get a pointer to the axis object through chart.get().
        /// Default: undefined
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The axis labels show the number for each tick.
        /// </summary>
        public ColorAxisLabels Labels { get; set; }

        /// <summary>
        /// The color of the line marking the axis itself.
        /// In styled mode, the line stroke is given in the .highcharts-axis-line or .highcharts-xaxis-line class.
        /// Default: #ccd6eb
        /// </summary>
        public Color? LineColor { get; set; }

        /// <summary>
        /// The triangular marker on a scalar color axis that points to the value of the hovered area. To disable the marker, set marker: null.
        /// </summary>
        public ColorAxisMarker Marker { get; set; }

        /// <summary>
        /// The maximum value of the axis in terms of map point values. If null, the max value is automatically calculated. If the endOnTick option is true, the max value might be rounded up.
        /// Default: undefined
        /// </summary>
        public Number? Max { get; set; }

        /// <summary>
        /// The color to represent the maximum of the color axis. Unless dataClasses or stops are set, the gradient ends at this value.
        /// If dataClasses are set, the color is based on minColor and maxColor unless a color is set for each data class, or the dataClassColor is set.
        /// Default: #003399
        /// </summary>
        public Color? MaxColor { get; set; }

        /// <summary>
        /// Padding of the max value relative to the length of the axis. A padding of 0.05 will make a 100px axis 5px longer.
        /// Default: 0
        /// </summary>
        public Number? MaxPadding { get; set; }

        /// <summary>
        /// The minimum value of the axis in terms of map point values. If null, the min value is automatically calculated. If the startOnTick option is true, the min value might be rounded down.
        /// Default: undefined
        /// </summary>
        public Number? Min { get; set; }

        /// <summary>
        /// The color to represent the minimum of the color axis. Unless dataClasses or stops are set, the gradient starts at this value.
        /// If dataClasses are set, the color is based on minColor and maxColor unless a color is set for each data class, or the dataClassColor is set.
        /// Default: #e6ebf5
        /// </summary>
        public Color? MinColor { get; set; }

        /// <summary>
        /// Color of the minor, secondary grid lines.
        /// In styled mode, the stroke width is given in the .highcharts-minor-grid-line class.
        /// Default: #f2f2f2
        /// </summary>
        public Color? MinorGridLineColor { get; set; }

        /// <summary>
        /// The dash or dot style of the minor grid lines. For possible values, see this demonstration.
        /// Default: Solid
        /// </summary>
        public DashStyles MinorGridLineDashStyle { get; set; }

        /// <summary>
        /// Width of the minor, secondary grid lines.
        /// Width of the minor, secondary grid lines.
        /// Default: 1
        /// </summary>
        public Number? MinorGridLineWidth { get; set; }

        /// <summary>
        /// Color for the minor tick marks.
        /// Default: #999999
        /// </summary>
        public Color? MinorTickColor { get; set; }

        /// <summary>
        /// Specific tick interval in axis units for the minor ticks. On a linear axis, if "auto", the minor tick interval is calculated as a fifth of the tickInterval. If null or undefined, minor ticks are not shown.
        /// On logarithmic axes, the unit is the power of the value. For example, setting the minorTickInterval to 1 puts one tick on each of 0.1, 1, 10, 100 etc. Setting the minorTickInterval to 0.1 produces 9 ticks between 1 and 10, 10 and 100 etc.
        /// If user settings dictate minor ticks to become too dense, they don't make sense, and will be ignored to prevent performance problems.
        /// Default: undefined
        /// </summary>
        public StringNumber? MinorTickInterval { get; set; }

        /// <summary>
        /// The pixel length of the minor tick marks.
        /// Default: 2
        /// </summary>
        public Number? MinorTickLength { get; set; }

        /// <summary>
        /// The position of the minor tick marks relative to the axis line. Can be one of inside and outside.
        /// Default: outside
        /// </summary>
        public TickPositions MinorTickPosition { get; set; }

        /// <summary>
        /// Enable or disable minor ticks. Unless minorTickInterval is set, the tick interval is calculated as a fifth of the tickInterval.
        /// On a logarithmic axis, minor ticks are laid out based on a best guess, attempting to enter approximately 5 minor ticks between each major tick.
        /// Prior to v6.0.0, ticks were unabled in auto layout by setting minorTickInterval to "auto".
        /// Default: false
        /// </summary>
        public bool? MinorTicks { get; set; }

        /// <summary>
        /// The pixel width of the minor tick mark.
        /// Default: 0
        /// </summary>
        public Number? MinorTickWidth { get; set; }

        /// <summary>
        /// Padding of the min value relative to the length of the axis. A padding of 0.05 will make a 100px axis 5px longer.
        /// Default: 0
        /// </summary>
        public Number? MinPadding { get; set; }

        /// <summary>
        /// Refers to the index in the panes array. Used for circular gauges and polar charts. When the option is not set then first pane will be used.
        /// Default: undefined
        /// </summary>
        public Number? Pane { get; set; }

        /// <summary>
        /// Whether to reverse the axis so that the highest number is closest to the origin. Defaults to false in a horizontal legend and true in a vertical legend, where the smallest value starts on top.
        /// Default: false
        /// </summary>
        public bool? Reversed { get; set; }

        /// <summary>
        /// This option determines how stacks should be ordered within a group. For example reversed xAxis also reverses stacks, so first series comes last in a group. To keep order like for non-reversed xAxis enable this option.
        /// Default: false
        /// </summary>
        public bool? ReversedStacks { get; set; }

        /// <summary>
        /// Whether to show the first tick label.
        /// Default: true
        /// </summary>
        public bool? ShowFirstLabel { get; set; }

        /// <summary>
        /// Whether to display the colorAxis in the legend.
        /// Default: true
        /// </summary>
        public bool? ShowInLegend { get; set; }

        /// <summary>
        /// Whether to show the last tick label. Defaults to true on cartesian charts, and false on polar charts.
        /// Default: true
        /// </summary>
        public bool? ShowLastLabel { get; set; }

        /// <summary>
        /// A soft maximum for the axis. If the series data maximum is less than this, the axis will stay at this maximum, but if the series data maximum is higher, the axis will flex to show all data.
        /// Default: undefined
        /// </summary>
        public Number? SoftMax { get; set; }

        /// <summary>
        /// A soft minimum for the axis. If the series data minimum is greater than this, the axis will stay at this minimum, but if the series data minimum is lower, the axis will flex to show all data.
        /// Default: undefined
        /// </summary>
        public Number? SoftMin { get; set; }

        /// <summary>
        /// For datetime axes, this decides where to put the tick between weeks. 0 = Sunday, 1 = Monday.
        /// Default: 1
        /// </summary>
        public WeekDays? StartOfWeek { get; set; }

        /// <summary>
        /// Whether to force the axis to start on a tick. Use this option with the maxPadding option to control the axis start.
        /// Default: true
        /// </summary>
        public bool? StartOnTick { get; set; }

        /// <summary>
        /// Color stops for the gradient of a scalar color axis. Use this in cases where a linear gradient between a minColor and maxColor is not sufficient. The stops is an array of tuples, where the first item is a float between 0 and 1 assigning the relative position in the gradient, and the second item is the color.
        /// Default: undefined
        /// </summary>
        public object[] Stops { get; set; }

        /// <summary>
        /// The amount of ticks to draw on the axis. This opens up for aligning the ticks of multiple charts or panes within a chart. This option overrides the tickPixelInterval option.
        /// This option only has an effect on linear axes. Datetime, logarithmic or category axes are not affected.
        /// Default: undefined
        /// </summary>
        public Number? TickAmount { get; set; }

        /// <summary>
        /// Color for the main tick marks.
        /// In styled mode, the stroke is given in the .highcharts-tick class.
        /// Default: #ccd6eb
        /// </summary>
        public Color? TickColor { get; set; }

        /// <summary>
        /// The interval of the tick marks in axis units. When null, the tick interval is computed to approximately follow the tickPixelInterval.
        /// Default: undefined
        /// </summary>
        public Number? TickInterval { get; set; }

        /// <summary>
        /// The pixel length of the main tick marks on the color axis.
        /// Default: 5
        /// </summary>
        public Number? TickLength { get; set; }

        /// <summary>
        /// For categorized axes only. If on the tick mark is placed in the center of the category, if between the tick mark is placed between categories. The default is between if the tickInterval is 1, else on.
        /// Default: between
        /// </summary>
        public TickmarkPlacements? TickmarkPlacement { get; set; }

        /// <summary>
        /// If tickInterval is null this option sets the approximate pixel interval of the tick marks.
        /// Default: 72
        /// </summary>
        public Number? TickPixelInterval { get; set; }

        /// <summary>
        /// The position of the major tick marks relative to the axis line. Can be one of inside and outside.
        /// Default: outside
        /// </summary>
        public TickPositions? TickPosition { get; set; }

        /// <summary>
        /// A callback function returning array defining where the ticks are laid out on the axis. This overrides the default behaviour of tickPixelInterval and tickInterval. The automatic tick positions are accessible through this.tickPositions and can be modified by the callback.
        /// Default: undefined
        /// </summary>
        [JsonFormatter("{0}")]
        public string TickPositioner { get; set; }

        /// <summary>
        /// An array defining where the ticks are laid out on the axis. This overrides the default behaviour of tickPixelInterval and tickInterval.
        /// Default: undefined
        /// </summary>
        public Number[] TickPositions { get; set; }

        /// <summary>
        /// The pixel width of the major tick marks.
        /// In styled mode, the stroke width is given in the .highcharts-tick class.
        /// Default: undefined
        /// </summary>
        public Number? TickWidth { get; set; }

        /// <summary>
        /// The type of interpolation to use for the color axis. Can be linear or logarithmic.
        /// Default: linear
        /// </summary>
        public ColorAxisTypes? Type { get; set; }

        /// <summary>
        /// Applies only when the axis type is category. When uniqueNames is true, points are placed on the X axis according to their names. If the same point name is repeated in the same or another series, the point is placed on the same X position as other points of the same name. When uniqueNames is false, the points are laid out in increasing X positions regardless of their names, and the X axis category will take the name of the last point in each position.
        /// Default: true
        /// </summary>
        public bool? uniqueNames { get; set; }
        /// <summary>
        /// Datetime axis only. An array determining what time intervals the ticks are allowed to fall on. 
        /// Each array item is an array where the first value is the time unit and the second value another 
        /// array of allowed multiples. Defaults to:
        /// Defaults to: 
        /// units: [[
        /// 	'millisecond', // unit name
        /// 	[1, 2, 5, 10, 20, 25, 50, 100, 200, 500] // allowed multiples
        /// ], [
        /// 	'second',
        /// 	[1, 2, 5, 10, 15, 30]
        /// ], [
        /// 	'minute',
        /// 	[1, 2, 5, 10, 15, 30]
        /// ], [
        /// 	'hour',
        /// 	[1, 2, 3, 4, 6, 8, 12]
        /// ], [
        /// 	'day',
        /// 	[1]
        /// ], [
        /// 	'week',
        /// 	[1]
        /// ], [
        /// 	'month',
        /// 	[1, 3, 6]
        /// ], [
        /// 	'year',
        /// 	null
        /// ]]
        /// </summary>
        public object[] Units { get; set; }

        /// <summary>
        /// Whether axis, including axis title, line, ticks and labels, should be visible.
        /// Default: true
        /// </summary>
        public bool? Visible { get; set; }
    }
}
