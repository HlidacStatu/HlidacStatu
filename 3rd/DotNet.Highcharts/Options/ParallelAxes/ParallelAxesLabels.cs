using DotNet.Highcharts.Attributes;
using DotNet.Highcharts.Enums;
using DotNet.Highcharts.Helpers;

namespace DotNet.Highcharts.Options.ParallelAxes
{
    /// <summary>
    /// The axis labels show the number or category for each tick.
    /// </summary>
    public class ParallelAxesLabels
    {
        /// <summary>
        /// What part of the string the given position is anchored to. Can be one of "left", "center" or "right". The exact position also depends on the labels.x setting.
        /// Angular gauges and solid gauges defaults to center.
        /// Default: center
        /// </summary>
        public HorizontalAligns? Align { get; set; }

        /// <summary>
        /// For horizontal axes, the allowed degrees of label rotation to prevent overlapping labels. If there is enough space, labels are not rotated. As the chart gets narrower, it will start rotating the labels -45 degrees, then remove every second label and try again with rotations 0 and -45 etc. Set it to false to disable rotation, which will cause the labels to word-wrap if possible.
        /// Default: [-45]
        /// </summary>
        public Number[] AutoRotation { get; set; }

        /// <summary>
        /// When each category width is more than this many pixels, we don't apply auto rotation. Instead, we lay out the axis label with word wrap. A lower limit makes sense when the label contains multiple short words that don't extend the available horizontal space for each label.
        /// Default: 80
        /// </summary>
        public Number? AutoRotationLimit { get; set; }

        /// <summary>
        /// Angular gauges and solid gauges only. The label's pixel distance from the perimeter of the plot area.
        /// Default: -25
        /// </summary>
        public Number? Distance { get; set; }

        /// <summary>
        /// Enable or disable the axis labels.
        /// Default: true
        /// </summary>
        public bool? Enabled { get; set; }

        /// <summary>
        /// A format string for the axis label.
        /// Default: {value}
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// Callback JavaScript function to format the label. The value is given by this.value. Additional properties for this are axis, chart, isFirst and isLast. The value of the default label formatter can be retrieved by calling this.axis.defaultLabelFormatter.call(this) within the function.
        /// Default: undefined
        /// </summary>
        [JsonFormatter("{0}")]
        public string Formatter { get; set; }

        /// <summary>
        /// How to handle overflowing labels on horizontal axis. If set to "allow", it will not be aligned at all. By default it "justify" labels inside the chart area. If there is room to move it, it will be aligned to the edge, else it will be removed.
        /// Default: justify
        /// </summary>
        public StringBool? Overflow { get; set; }

        /// <summary>
        /// The pixel padding for axis labels, to ensure white space between them.
        /// Default: 5
        /// </summary>
        public Number? Padding { get; set; }

        /// <summary>
        /// Defines how the labels are be repositioned according to the 3D chart orientation.
        /// 'offset': Maintain a fixed horizontal/vertical distance from the tick marks, despite the chart orientation. This is the backwards compatible behavior, and causes skewing of X and Z axes.
        /// 'chart': Preserve 3D position relative to the chart. This looks nice, but hard to read if the text isn't forward-facing.
        /// 'flap': Rotated text along the axis to compensate for the chart orientation. This tries to maintain text as legible as possible on all orientations.
        /// 'ortho': Rotated text along the axis direction so that the labels are orthogonal to the axis. This is very similar to 'flap', but prevents skewing the labels (X and Y scaling are still present).
        /// Default: offset
        /// </summary>
        public string Position3d { get; set; }

        /// <summary>
        /// Whether to reserve space for the labels. By default, space is reserved for the labels in these cases:
        /// On all horizontal axes.
        /// On vertical axes if label.align is right on a left-side axis or left on a right-side axis.
        /// On vertical axes if label.align is center.
        /// This can be turned off when for example the labels are rendered inside the plot area instead of outside.
        /// Default: false
        /// </summary>
        public bool? ReserveSpace { get; set; }

        /// <summary>
        /// Rotation of the labels in degrees.
        /// Default: 0
        /// </summary>
        public Number? Rotation { get; set; }

        /// <summary>
        /// If enabled, the axis labels will skewed to follow the perspective.
        /// This will fix overlapping labels and titles, but texts become less legible due to the distortion.
        /// The final appearance depends heavily on labels.position3d.
        /// Default: false
        /// </summary>
        public bool? Skew3d { get; set; }

        /// <summary>
        /// Horizontal axes only. The number of lines to spread the labels over to make room or tighter labels.
        /// Default: undefined
        /// </summary>
        public Number? StaggerLines { get; set; }

        /// <summary>
        /// To show only every n'th label on the axis, set the step to n. Setting the step to 2 shows every other label.
        /// By default, the step is calculated automatically to avoid overlap. To prevent this, set it to 1. This usually only happens on a category axis, and is often a sign that you have chosen the wrong axis type.
        /// Read more at Axis docs => What axis should I use?
        /// Default: undefined
        /// </summary>
        public Number? Step { get; set; }

        /// <summary>
        /// CSS styles for the label. Use whiteSpace: 'nowrap' to prevent wrapping of category labels. Use textOverflow: 'none' to prevent ellipsis (dots).
        /// In styled mode, the labels are styled with the .highcharts-axis-labels class.
        /// Default: {"color": "#666666", "cursor": "default", "fontSize": "11px"}
        /// </summary>
        [JsonFormatter("{{ {0} }}")]
        public string Style { get; set; }

        /// <summary>
        /// Whether to use HTML to render the labels.
        /// Default: false
        /// </summary>
        public bool? UseHTML { get; set; }

        /// <summary>
        /// The x position offset of the label relative to the tick position on the axis. Defaults to -15 for left axis, 15 for right axis.
        /// Default: 0
        /// </summary>
        public Number? X { get; set; }

        /// <summary>
        /// The y position offset of the label relative to the tick position on the axis.
        /// Default: 4
        /// </summary>
        public Number? Y { get; set; }

        /// <summary>
        /// The Z index for the axis labels.
        /// Default: 7
        /// </summary>
        public Number? ZIndex { get; set; }
    }
}
