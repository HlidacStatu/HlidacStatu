using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotNet.Highcharts.Attributes;

namespace DotNet.Highcharts.Helpers
{

    /// <summary>
    /// Defines on what value to start the series
    /// </summary>
    public class PointStart
    {
        /// <summary>
        /// Constructor with start point as date time
        /// </summary>
        /// <param name="pointStart"></param>
        public PointStart(DateTime pointStart) { PointStartDate = pointStart; }

        /// <summary>
        /// Constructor with start point as number
        /// </summary>
        /// <param name="pointStart"></param>
        public PointStart(Number pointStart) { PointStartNumber = pointStart; }

        /// <summary>
        /// Date start point
        /// </summary>
        [Name("PointStart")]
        public DateTime? PointStartDate { get; private set; }

        /// <summary>
        /// Number start point
        /// </summary>
        [Name("PointStart")]
        public Number? PointStartNumber { get; private set; }
    }
}
