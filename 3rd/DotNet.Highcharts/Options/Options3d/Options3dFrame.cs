using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotNet.Highcharts.Helpers;

namespace DotNet.Highcharts.Options.Options3d
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// Provides the option to draw a frame around the charts by defining a bottom, front and back panel.
    /// </summary>
    public class Options3dFrame
    {
        /// <summary>
        /// The back side of the frame around a 3D chart.
        /// </summary>
        public Options3dSpecificFrameSide Back { get; set; }

        /// <summary>
        /// The bottom of the frame around a 3D chart.
        /// </summary>
        public Options3dSpecificFrameSide Bottom { get; set; }

        /// <summary>
        /// The front of the frame around a 3D chart.
        /// </summary>
        public Options3dSpecificFrameSide Front { get; set; }

        /// <summary>
        /// The left side of the frame around a 3D chart.
        /// </summary>
        public Options3dSpecificFrameSide Left { get; set; }

        /// <summary>
        /// The right of the frame around a 3D chart.
        /// </summary>
        public Options3dSpecificFrameSide Right { get; set; }

        /// <summary>
        /// Note: As of v5.0.12, frame.left or frame.right should be used instead.
        /// The side for the frame around a 3D chart.
        /// </summary>
        public Options3dFrameSide Side { get; set; }

        /// <summary>
        /// General pixel thickness for the frame faces.
        /// Default: 1
        /// </summary>
        public Number? Size { get; set; }

        /// <summary>
        /// The top of the frame around a 3D chart.
        /// </summary>
        public Options3dSpecificFrameSide Top { get; set; }

        /// <summary>
        /// Whether the frames are visible.
        /// Default: default
        /// </summary>
        public string Visible { get; set; }
    }
}
