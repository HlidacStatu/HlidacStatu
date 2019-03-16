using DotNet.Highcharts.Attributes;
using DotNet.Highcharts.Enums;
using DotNet.Highcharts.Helpers;

namespace DotNet.Highcharts.Options.YAxis
{
	public class YAxisLabels
	{
		/// <summary>
		/// What part of the string the given position is anchored to.  Can be one of <code>'left'</code>, <code>'center'</code> or <code>'right'</code>.
		/// Default: right
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
		/// A <a href='http://docs.highcharts.com#formatting'>format string</a> for the axis label. 
		/// </summary>
		public string Format { get; set; }

		/// <summary>
		/// Callback JavaScript function to format the label. The value is  given by <code>this.value</code>. Additional properties for <code>this</code> are <code>axis</code>, <code>chart</code>, <code>isFirst</code> and <code>isLast</code>. Defaults to: <pre>function() { return this.value;}</pre>
		/// </summary>
		[JsonFormatter("{0}")]
		public string Formatter { get; set; }

		/// <summary>
		/// How to handle overflowing labels on horizontal axis. Can be undefined or 'justify'. If 'justify', labels will not render outside the plot area. If there is room to move it, it will be aligned to the edge, else it will be removed.
		/// </summary>
		public string Overflow { get; set; }

        /// <summary>
        /// The pixel padding for axis labels, to ensure white space between them.
        /// Default: 5
        /// </summary>
        public Number? Padding { get; set; }

	    /// <summary>
	    /// Defines how the labels are be repositioned according to the 3D chart orientation.<para />
	    /// <ul>
	    /// <li>'offset': Maintain a fixed horizontal/vertical distance from the tick marks, despite the chart orientation. This is the backwards compatible behavior, and causes skewing of X and Z axes.</li><para />
	    /// <li>'chart': Preserve 3D position relative to the chart. This looks nice, but hard to read if the text isn't forward-facing.</li><para />
	    /// <li>'flap': Rotated text along the axis to compensate for the chart orientation. This tries to maintain text as legible as possible on all orientations.</li><para />
	    /// <li>'ortho': Rotated text along the axis direction so that the labels are orthogonal to the axis. This is very similar to 'flap', but prevents skewing the labels (X and Y scaling are still present)</li><para />
	    /// </ul>
	    /// Default: offset
	    /// </summary>
	    public string Position3d { get; set; }

	    /// <summary>
	    /// Whether to reserve space for the labels. By default, space is reserved for the labels in these cases:<para />
	    /// - On all horizontal axes.<para />
	    /// - On vertical axes if label.align is right on a left-side axis or left on a right-side axis.<para />
	    /// - On vertical axes if label.align is center.<para />
	    /// This can be turned off when for example the labels are rendered inside the plot area instead of outside.<para />
	    /// Default: undefined
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
        /// Horizontal axes only. The number of lines to spread the labels over to make room or tighter labels.  .
        /// </summary>
        public Number? StaggerLines { get; set; }

		/// <summary>
		/// To show only every <em>n</em>'th label on the axis, set the step to <em>n</em>. Setting the step to 2 shows every other label.
		/// </summary>
		public Number? Step { get; set; }

		/// <summary>
		/// CSS styles for the label. Defaults to:<pre>style: { color: '#6D869F', fontWeight: 'bold'}</pre>
		/// </summary>
		[JsonFormatter("{{ {0} }}")]
		public string Style { get; set; }

		/// <summary>
		/// Whether to use HTML to render the labels. Using HTML allows advanced formatting, images and reliable bi-directional text rendering. Note that exported images won't respect the HTML, and that HTML won't respect Z-index settings.
		/// Default: false
		/// </summary>
		public bool? UseHTML { get; set; }

		/// <summary>
		/// The x position offset of the label relative to the tick position on the axis.
		/// Default: -8
		/// </summary>
		public Number? X { get; set; }

		/// <summary>
		/// The y position offset of the label relative to the tick position on the axis.
		/// Default: 3
		/// </summary>
		public Number? Y { get; set; }

		/// <summary>
		/// The Z index for the axis labels.
		/// Default: 7
		/// </summary>
		public Number? ZIndex { get; set; }
	}

}