using DotNet.Highcharts.Attributes;

namespace DotNet.Highcharts.Options.PlotOptions
{
	/// <summary>
	/// Events for each single point
	/// </summary>
	public class PlotOptionsArearangePointEvents
	{
		/// <summary>
		/// <p></p>Fires when a point is clicked. The <code>this</code> keyword refers to the  point object itself. One parameter, <code>event</code>, is passed to the function. This contains common event information based on jQuery or MooTools depending on  which library is used as the base for Highcharts.<p></p> <p>If the <code>series.allowPointSelect</code> option is true, the default action for the point's click event is to toggle the point's select state. Returning <code>false</code> cansels this action.</p>
		/// </summary>
		[JsonFormatter("{0}")]
		public string Click { get; set; }

		/// <summary>
		/// Fires when the mouse leaves the area close to the point. The <code>this</code> keyword refers to the  point object itself. One parameter, <code>event</code>, is passed to the function. This contains common event information based on jQuery or MooTools depending on  which library is used as the base for Highcharts.
		/// </summary>
		[JsonFormatter("{0}")]
		public string MouseOut { get; set; }

		/// <summary>
		/// Fires when the mouse enters the area close to the point. The <code>this</code> keyword refers to the  point object itself. One parameter, <code>event</code>, is passed to the function. This contains common event information based on jQuery or MooTools depending on  which library is used as the base for Highcharts.
		/// </summary>
		[JsonFormatter("{0}")]
		public string MouseOver { get; set; }

		/// <summary>
		/// Fires when the point is removed using the <code>.remove()</code> method. The <code>this</code> keyword refers to the  point object itself. One parameter, <code>event</code>, is passed to the function. Returning <code>false</code> cancels the operation.
		/// </summary>
		[JsonFormatter("{0}")]
		public string Remove { get; set; }

		/// <summary>
		/// Fires when the point is selected either programatically or following a click on the point. The <code>this</code> keyword refers to the  point object itself. One parameter, <code>event</code>, is passed to the function. Returning <code>false</code> cancels the operation.
		/// </summary>
		[JsonFormatter("{0}")]
		public string Select { get; set; }

		/// <summary>
		/// Fires when the point is unselected either programatically or following a click on the point. The <code>this</code> keyword refers to the  point object itself. One parameter, <code>event</code>, is passed to the function. Returning <code>false</code> cancels the operation.
		/// </summary>
		[JsonFormatter("{0}")]
		public string Unselect { get; set; }

		/// <summary>
		/// Fires when the point is updated programmatically through the <code>.update()</code> method. The <code>this</code> keyword refers to the  point object itself. One parameter, <code>event</code>, is passed to the function. The  new point options can be accessed through <code>event.options</code>. Returning <code>false</code> cancels the operation.
		/// </summary>
		[JsonFormatter("{0}")]
		public string Update { get; set; }

	}

}