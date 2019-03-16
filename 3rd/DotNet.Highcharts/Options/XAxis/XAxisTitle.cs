using System;
using DotNet.Highcharts.Attributes;
using DotNet.Highcharts.Enums;
using DotNet.Highcharts.Helpers;

namespace DotNet.Highcharts.Options.XAxis
{
	/// <summary>
	/// The axis title, showing next to the axis line.
	/// </summary>
	public class XAxisTitle
	{
		/// <summary>
		/// Alignment of the title relative to the axis values. Possible values are 'low', 'middle' or 'high'.
		/// Default: middle
		/// </summary>
		public AxisTitleAligns? Align { get; set; }

		[Obsolete("Deprecated. Set the <code>text</code> to <code>null</code> to disable the title.")]
		public string Enabled { get; set; }

		/// <summary>
		/// The pixel distance between the axis labels or line and the title. Defaults to 0 for horizontal axes, 10 for vertical
		/// </summary>
		public Number? Margin { get; set; }

		/// <summary>
		/// The distance of the axis title from the axis line. By default, this distance is  computed from the offset width of the labels, the labels' distance from  the axis and the title's margin. However when the offset option is set, it overrides all this.
		/// </summary>
		public Number? Offset { get; set; }

        /// <summary>
        /// Defines how the title is repositioned according to the 3D chart orientation.
        /// 'offset': Maintain a fixed horizontal/vertical distance from the tick marks, despite the chart orientation. This is the backwards compatible behavior, and causes skewing of X and Z axes.
        /// 'chart': Preserve 3D position relative to the chart. This looks nice, but hard to read if the text isn't forward-facing.
        /// 'flap': Rotated text along the axis to compensate for the chart orientation. This tries to maintain text as legible as possible on all orientations.
        /// 'ortho': Rotated text along the axis direction so that the labels are orthogonal to the axis. This is very similar to 'flap', but prevents skewing the labels (X and Y scaling are still present).
        /// null: Will use the config from labels.position3d
        /// Default: null
        /// </summary>
        public string Position3d { get; set; }

        /// <summary>
        /// Whether to reserve space for the title when laying out the axis.
        /// Default: true
        /// </summary>
        public bool? ReserveSpace { get; set; }

		/// <summary>
		/// The rotation of the text in degrees. 0 is horizontal, 270 is vertical reading from bottom to top.
		/// Default: 0
		/// </summary>
		public Number? Rotation { get; set; }

        /// <summary>
        /// If enabled, the axis title will skewed to follow the perspective.
        /// This will fix overlapping labels and titles, but texts become less legible due to the distortion.
        /// The final appearance depends heavily on title.position3d.
        /// A null value will use the config from labels.skew3d.
        /// Default: null
        /// </summary>
        public bool? Skew3d { get; set; }

		/// <summary>
		/// CSS styles for the title. When titles are rotated they are rendered using vector graphic techniques and not all styles are applicable. Defaults to: <pre>style: { color: '#6D869F', fontWeight: 'bold'}</pre>
		/// </summary>
		[JsonFormatter("{{ {0} }}")]
		public string Style { get; set; }

		/// <summary>
		/// The actual text of the axis title. It can contain basic HTML text markup like &lt;b&gt;, &lt;i&gt; and spans with style.
		/// </summary>
		public string Text { get; set; }

        /// <summary>
        /// Alignment of the text, can be "left", "right" or "center". Default alignment depends on the title.align:
        /// Horizontal axes:
        /// - for align = "low", textAlign is set to left
        /// - for align = "middle", textAlign is set to center
        /// - for align = "high", textAlign is set to right
        /// Vertical axes:
        /// - for align = "low" and opposite = true, textAlign is set to right
        /// - for align = "low" and opposite = false, textAlign is set to left
        /// - for align = "middle", textAlign is set to center
        /// - for align = "high" and opposite = true textAlign is set to left
        /// - for align = "high" and opposite = false textAlign is set to right
        /// Default: undefined
        /// </summary>
        public HorizontalAligns? TextAlign { get; set; }

        /// <summary>
        /// Whether to use HTML to render the axis title.
        /// Default: false
        /// </summary>
        public bool? UseHTML { get; set; }

        /// <summary>
        /// Horizontal pixel offset of the title position.
        /// Default: 0
        /// </summary>
        public Number? X { get; set; }

        /// <summary>
        /// Vertical pixel offset of the title position.
        /// Default: undefined
        /// </summary>
        public Number? Y { get; set; }
	}

}