using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNet.Highcharts.Options.Boost
{
    /// <summary>
    /// Debugging options for boost. Useful for benchmarking, and general timing.
    /// </summary>
    public class BoostDebug
    {
        /// <summary>
        /// Show the number of points skipped through culling.
        /// When set to true, the number of points skipped in series processing is outputted. Points are skipped if they are closer than 1 pixel from each other.
        /// Default: false
        /// </summary>
        public bool? ShowSkipSummary { get; set; }

        /// <summary>
        /// Time the WebGL to SVG buffer copy
        /// After rendering, the result is copied to an image which is injected into the SVG.
        /// If this property is set to true, the time it takes for the buffer copy to complete is outputted.
        /// Default: false
        /// </summary>
        public bool? TimeBuggerCopy { get; set; }


        // ReSharper disable once InconsistentNaming
        /// <summary>
        /// Time the building of the k-d tree.
        /// This outputs the time spent building the k-d tree used for markers etc.
        /// Note that the k-d tree is built async, and runs post-rendering. Following, it does not affect the performance of the rendering itself.
        /// Default: false
        /// </summary>
        public bool? TimeKDTRee { get; set; }

        /// <summary>
        /// Time the series rendering.
        /// This outputs the time spent on actual rendering in the console when set to true.
        /// Default: false
        /// </summary>
        public bool? TimeRendering { get; set; }

        /// <summary>
        /// Time the series processing.
        /// This outputs the time spent on transforming the series data to vertex buffers when set to true.
        /// Default: false
        /// </summary>
        public bool? TimeSeriesProcessing { get; set; }

        /// <summary>
        /// Time the the WebGL setup.
        /// This outputs the time spent on setting up the WebGL context, creating shaders, and textures.
        /// Default: false
        /// </summary>
        public bool? TimeSetup { get; set; }
    }
}
