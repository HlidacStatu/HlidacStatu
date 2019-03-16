﻿using DotNet.Highcharts.Attributes;
using DotNet.Highcharts.Enums;
using DotNet.Highcharts.Helpers;
using System.Drawing;

namespace DotNet.Highcharts.Options.ParallelAxes
{
    public class ParallelAxes
    {
        /// <summary>
        /// When using multiple axis, the ticks of two or more opposite axes will automatically be 
        /// aligned by adding ticks to the axis or axes with the least ticks, as if tickAmount were specified.
        /// This can be prevented by setting alignTicks to false. If the grid lines look messy, it's 
        /// a good idea to hide them for the secondary axis by setting gridLineWidth to 0.
        /// If startOnTick or endOnTick in an Axis options are set to false, then the alignTicks will be disabled for the Axis.
        /// Disabled for logarithmic axes.
        /// Default: true
        /// </summary>
        public bool? AlignTicks { get; set; }

        /// <summary>
        /// Whether to allow decimals in this axis' ticks. When counting integers, like persons or 
        /// hits on a web page, decimals should be avoided in the labels.
        /// Default: true
        /// </summary>
        public bool? AllowDecimals { get; set; }

        /// <summary>
        /// If categories are present for the xAxis, names are used instead of numbers for that axis. 
        /// Since Highcharts 3.0, categories can also be extracted by giving each point a name and 
        /// setting axis type to category. However, if you have multiple series, best practice remains 
        /// defining the categories array.
        /// Default: undefined
        /// </summary>
        public string[] Categories { get; set; }

        /// <summary>
        /// The highest allowed value for automatically computed axis extremes.
        /// Default: undefined
        /// </summary>
        public Number? Ceiling { get; set; }

        /// <summary>
        /// A class name that opens for styling the axis by CSS, especially in Highcharts styled mode. 
        /// The class name is applied to group elements for the grid, axis elements and labels.
        /// Default: undefined
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// Configure a crosshair that follows either the mouse pointer or the hovered point.
        /// In styled mode, the crosshairs are styled in the .highcharts-crosshair, .highcharts-crosshair-thin or .highcharts-xaxis-category classes.
        /// </summary>
        public ParallelAxesCrosshair Crosshair { get; set; }

        /// <summary>
        /// For a datetime axis, the scale will automatically adjust to the appropriate unit. This member
        ///  gives the default string representations used for each unit. For intermediate values, different
        ///  units may be used, for example the day unit can be used on midnight and hour unit be used for
        ///  intermediate values on the same axis. For an overview of the replacement codes, see dateFormat.  
        /// </summary>
        public DateTimeLabel DateTimeLabelFormats { get; set; }

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
        public ParallelAxesEvents Events { get; set; }

        /// <summary>
        /// The lowest allowed value for automatically computed axis extremes.
        /// Default: undefined
        /// </summary>
        public Number? Floor { get; set; }

        /// <summary>
        /// The Z index of the grid lines.
        /// Default: 1
        /// </summary>
        public Number? GridZIndex { get; set; }

        /// <summary>
        /// The axis labels show the number or category for each tick.
        /// </summary>
        public ParallelAxesLabels Labels { get; set; }

        /// <summary>
        /// In styled mode, the line stroke is given in the .highcharts-axis-line or .highcharts-xaxis-line class.
        /// Default: #ccd6eb
        /// </summary>
        public Color? LineColor { get; set; }

        /// <summary>
        /// In styled mode, the stroke width is given in the .highcharts-axis-line or .highcharts-xaxis-line class.
        /// Default: 1
        /// </summary>
        public Number? LineWidth { get; set; }

        /// <summary>
        /// Index of another axis that this axis is linked to. When an axis is linked to a master axis,
        ///  it will take the same extremes as the master, but as assigned by min or max or by setExtremes.
        ///  It can be used to show additional info, or to ease reading the chart by duplicating the scales.
        /// Default: undefined
        /// </summary>
        public Number? LinkedTo { get; set; }

        /// <summary>
        /// The maximum value of the axis. If null, the max value is automatically calculated.
        /// If the endOnTick option is true, the max value might be rounded up.
        /// If a tickAmount is set, the axis may be extended beyond the set max in order to reach the 
        /// given number of ticks. The same may happen in a chart with multiple axes, determined by 
        /// chart. alignTicks, where a tickAmount is applied internally.
        /// Default: undefined
        /// </summary>
        public Number? Max { get; set; }

        /// <summary>
        /// Padding of the max value relative to the length of the axis. A padding of 0.05 will 
        /// make a 100px axis 5px longer. This is useful when you don't want the highest data value 
        /// to appear on the edge of the plot area. When the axis' max option is set or a max extreme 
        /// is set using axis.setExtremes(), the maxPadding will be ignored.
        /// Default: 0.05
        /// </summary>
        public Number? MaxPadding { get; set; }

        /// <summary>
        /// The minimum value of the axis. If null the min value is automatically calculated.
        /// If the startOnTick option is true (default), the min value might be rounded down.
        /// The automatically calculated minimum value is also affected by floor, softMin, minPadding, minRange as well as series.threshold and series.softThreshold.
        /// Default: undefined
        /// </summary>
        public Number? Min { get; set; }

        /// <summary>
        /// Color for the minor tick marks.
        /// Default: #999999
        /// </summary>
        public Color? MinorTickColor { get; set; }

        /// <summary>
        /// Specific tick interval in axis units for the minor ticks. On a linear axis, if "auto", 
        /// the minor tick interval is calculated as a fifth of the tickInterval. If null or undefined,
        ///  minor ticks are not shown.
        /// On logarithmic axes, the unit is the power of the value. For example, setting the 
        /// minorTickInterval to 1 puts one tick on each of 0.1, 1, 10, 100 etc. Setting the 
        /// minorTickInterval to 0.1 produces 9 ticks between 1 and 10, 10 and 100 etc.
        /// If user settings dictate minor ticks to become too dense, they don't make sense, and will 
        /// be ignored to prevent performance problems.
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
        /// Enable or disable minor ticks. Unless minorTickInterval is set, the tick interval
        ///  is calculated as a fifth of the tickInterval.
        /// On a logarithmic axis, minor ticks are laid out based on a best guess, attempting 
        /// to enter approximately 5 minor ticks between each major tick.
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
        /// Padding of the min value relative to the length of the axis. A padding of 0.05 will make 
        /// a 100px axis 5px longer. This is useful when you don't want the lowest data value to appear
        ///  on the edge of the plot area. When the axis' min option is set or a max extreme is set using
        ///  axis.setExtremes(), the maxPadding will be ignored.
        /// Default: 0.05
        /// </summary>
        public Number? MinPadding { get; set; }

        /// <summary>
        /// The minimum range to display on this axis. The entire axis will not be allowed to span 
        /// over a smaller interval than this. For example, for a datetime axis the main unit is 
        /// milliseconds. If minRange is set to 3600000, you can't zoom in more than to one hour.
        /// The default minRange for the x axis is five times the smallest interval between any of the data points.
        /// On a logarithmic axis, the unit for the minimum range is the power. So a minRange of 1 means that the axis can be zoomed to 10-100, 100-1000, 1000-10000 etc.
        /// Note that the minPadding, maxPadding, startOnTick and endOnTick settings also affect how the extremes of the axis are computed.
        /// Default: undefined
        /// </summary>
        public Number? MinRange { get; set; }

        /// <summary>
        /// The minimum tick interval allowed in axis values. For example on zooming in on an axis
        ///  with daily data, this can be used to prevent the axis from showing hours. Defaults to 
        /// the closest distance between two points on the axis.
        /// Default: undefined
        /// </summary>
        public Number? MinTickInterval { get; set; }

        /// <summary>
        /// The distance in pixels from the plot area to the axis line. A positive offset moves the axis
        ///  with it's line, labels and ticks away from the plot area. This is typically used when two 
        /// or more axes are displayed on the same side of the plot. With multiple axes the offset is 
        /// dynamically adjusted to avoid collision, this can be overridden by setting offset explicitly.
        /// Default: 0
        /// </summary>
        public Number? Offset { get; set; }

        /// <summary>
        /// Whether to display the axis on the opposite side of the normal. The normal is on the left 
        /// side for vertical axes and bottom for horizontal, so the opposite sides will be right and 
        /// top respectively. This is typically used with dual or multiple axes.
        /// Default: false
        /// </summary>
        public bool? Opposite { get; set; }

        /// <summary>
        /// Refers to the index in the panes array. Used for circular gauges and polar charts. When the option is not set then first pane will be used.
        /// Default: undefined
        /// </summary>
        public Number? Pane { get; set; }

        /// <summary>
        /// Whether to reverse the axis so that the highest number is closest to the origin.
        /// Default: false
        /// </summary>
        public bool? Reversed { get; set; }

        /// <summary>
        /// If true, the first series in a stack will be drawn on top in a positive, non-reversed Y axis. If false, the first series is in the base of the stack.
        /// Default: true
        /// </summary>
        public bool? ReversedStacks { get; set; }

        /// <summary>
        /// Whether to show the axis line and title when the axis has no data.
        /// Default: true
        /// </summary>
        public bool? ShowEmpty { get; set; }

        /// <summary>
        /// Whether to show the first tick label.
        /// Default: true
        /// </summary>
        public bool? ShowFirstLabel { get; set; }

        /// <summary>
        /// Whether to show the last tick label. Defaults to true on cartesian charts, and false on polar charts.
        /// Default: true
        /// </summary>
        public bool? ShowLastLabel { get; set; }

        /// <summary>
        /// A soft maximum for the axis. If the series data maximum is less than this, the axis will 
        /// stay at this maximum, but if the series data maximum is higher, the axis will flex to show all data.
        /// Note: The series.softThreshold option takes precedence over this option.
        /// Default: undefined
        /// </summary>
        public Number? SoftMax { get; set; }

        /// <summary>
        /// A soft minimum for the axis. If the series data minimum is greater than this, the axis will 
        /// stay at this minimum, but if the series data minimum is lower, the axis will flex to show all data.
        /// Note: The series.softThreshold option takes precedence over this option.
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
        /// The amount of ticks to draw on the axis. This opens up for aligning the ticks of multiple charts 
        /// or panes within a chart. This option overrides the tickPixelInterval option.
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
        /// The interval of the tick marks in axis units. When undefined, the tick interval is computed 
        /// to approximately follow the tickPixelInterval on linear and datetime axes. On categorized axes,
        ///  a undefined tickInterval will default to 1, one category. Note that datetime axes are based on
        ///  milliseconds, so for example an interval of one day is expressed as 24 * 3600 * 1000.
        /// On logarithmic axes, the tickInterval is based on powers, so a tickInterval of 1 means one tick
        ///  on each of 0.1, 1, 10, 100 etc. A tickInterval of 2 means a tick of 0.1, 10, 1000 etc. A 
        /// tickInterval of 0.2 puts a tick on 0.1, 0.2, 0.4, 0.6, 0.8, 1, 2, 4, 6, 8, 10, 20, 40 etc.
        /// If the tickInterval is too dense for labels to be drawn, Highcharts may remove ticks.
        /// If the chart has multiple axes, the alignTicks option may interfere with the tickInterval setting.
        /// Default: undefined
        /// </summary>
        public Number? TickInterval { get; set; }

        /// <summary>
        /// The pixel length of the main tick marks.
        /// Default: 10
        /// </summary>
        public Number? TickLength { get; set; }

        /// <summary>
        /// For categorized axes only. If on the tick mark is placed in the center of the category, 
        /// if between the tick mark is placed between categories. The default is between if the tickInterval is 1, else on.
        /// Default: between
        /// </summary>
        public TickmarkPlacements TickmarkPlacement { get; set; }

        /// <summary>
        /// If tickInterval is null this option sets the approximate pixel interval of the tick marks.
        ///  Not applicable to categorized axis.
        /// The tick interval is also influenced by the minTickInterval option, that, by default prevents 
        /// ticks from being denser than the data points.
        /// Default: 72
        /// </summary>
        public Number? TickPixelInterval { get; set; }

        /// <summary>
        /// The position of the major tick marks relative to the axis line. Can be one of inside
        ///  and outside.
        /// Default: outside
        /// </summary>
        public TickPositions? TickPosition { get; set; }

        /// <summary>
        /// A callback function returning array defining where the ticks are laid out on the axis.
        ///  This overrides the default behaviour of tickPixelInterval and tickInterval. The automatic
        ///  tick positions are accessible through this.tickPositions and can be modified by the callback.
        /// Default: undefined
        /// </summary>
        [JsonFormatter("{0}")]
        public string TickPositioner { get; set; }

        /// <summary>
        /// An array defining where the ticks are laid out on the axis. This overrides the default
        ///  behaviour of tickPixelInterval and tickInterval.
        /// Default: undefined
        /// </summary>
        public Number[] TickPositions { get; set; }

        /// <summary>
        /// The pixel width of the major tick marks.
        /// Default: 0
        /// </summary>
        public Number? TickWidth { get; set; }

        /// <summary>
        /// Titles for yAxes are taken from xAxis.categories. All options for xAxis.labels applies to parallel
        ///  coordinates titles. For example, to style categories, use xAxis.labels.style.
        /// </summary>
        public ParallelAxesTitle Title { get; set; }

        /// <summary>
        /// Parallel coordinates only. Format that will be used for point.y and available in tooltip.pointFormat 
        /// as {point.formattedValue}. If not set, {point.formattedValue} will use other options, in this order:
        /// 1. yAxis.labels.format will be used if set
        /// 2. if yAxis is a category, then category name will be displayed
        /// 3. if yAxis is a datetime, then value will use the same format as yAxis labels
        /// 4. if yAxis is linear/logarithmic type, then simple value will be used
        /// Default: undefined
        /// </summary>
        public string TooltipValueFormat { get; set; }

        /// <summary>
        /// The type of axis. Can be one of linear, logarithmic, datetime or category. In a datetime axis, 
        /// the numbers are given in milliseconds, and tick marks are placed on appropriate values like full 
        /// hours or days. In a category axis, the point names of the chart's series are used for categories, 
        /// if not a categories array is defined.
        /// Default: linear
        /// </summary>
        public AxisTypes Type { get; set; }

        /// <summary>
        /// Applies only when the axis type is category. When uniqueNames is true, points are placed on 
        /// the X axis according to their names. If the same point name is repeated in the same or another 
        /// series, the point is placed on the same X position as other points of the same name. When uniqueNames 
        /// is false, the points are laid out in increasing X positions regardless of their names, and 
        /// the X axis category will take the name of the last point in each position.
        /// Default: true
        /// </summary>
        public bool? UniqueNames { get; set; }

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
