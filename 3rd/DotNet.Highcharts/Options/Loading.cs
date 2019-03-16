using System;
using System.Drawing;
using DotNet.Highcharts.Enums;
using DotNet.Highcharts.Attributes;
using DotNet.Highcharts.Helpers;

namespace DotNet.Highcharts.Options
{
	/// <summary>
	/// The loading options control the appearance of the loading screen that covers the  plot area on chart operations. This screen only appears after an explicit call to <code>chart.showLoading()</code>. It is a utility for developers to communicate to the end user that something is going on, for example while retrieving new data via an XHR connection. The 'Loading...' text itself is not part of this configuration object, but part of the <code>lang</code> object.
	/// </summary>
	public class Loading
	{
		/// <summary>
		/// The duration in milliseconds of the fade out effect.
		/// Default: 100
		/// </summary>
		public Number? HideDuration { get; set; }

		/// <summary>
		/// CSS styles for the loading label <code>span</code>. Defaults to:<pre>labelStyle: { fontWeight: 'bold', position: 'relative', top: '1em'}</pre>
		/// </summary>
		[JsonFormatter("{{ {0} }}")]
		public string LabelStyle { get; set; }

		/// <summary>
		/// The duration in milliseconds of the fade in effect.
		/// Default: 100
		/// </summary>
		public Number? ShowDuration { get; set; }

		/// <summary>
		/// CSS styles for the loading screen that covers the plot area. Defaults to:<pre>style: { position: 'absolute', backgroundColor: 'white', opacity: 0.5, textAlign: 'center'}</pre>
		/// </summary>
		[JsonFormatter("{{ {0} }}")]
		public string Style { get; set; }

	}

}