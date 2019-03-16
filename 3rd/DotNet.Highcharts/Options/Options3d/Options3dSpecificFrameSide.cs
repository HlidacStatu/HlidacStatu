using DotNet.Highcharts.Helpers;

namespace DotNet.Highcharts.Options.Options3d
{
    // ReSharper disable once InconsistentNaming
    public class Options3dSpecificFrameSide : Options3dFrameSide
    {
        /// <summary>
        /// Whether to display the frame. Possible values are true, false, "auto" to display only the frames behind the data, and "default" to display faces behind the data based on the axis layout, ignoring the point of view.
        /// Default: default
        /// </summary>
        public StringBool Visible { get; set; }
    }
}
