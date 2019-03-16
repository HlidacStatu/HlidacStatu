using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace HlidacStatu.Web.Framework
{
	public static class RoutingExtensions
	{
		
    
		public static RouteValueDictionary Merge(this RouteValueDictionary self, IEnumerable<KeyValuePair<string, object>> routeValues)
		{
			var result = new RouteValueDictionary(self);

			if (routeValues != null)
				foreach (var pair in routeValues)
					result[pair.Key] = pair.Value;

			return result;
		}
	}
}