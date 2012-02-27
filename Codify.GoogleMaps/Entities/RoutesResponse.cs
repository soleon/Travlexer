using System.Collections.Generic;
using Newtonsoft.Json;

namespace Codify.GoogleMaps.Entities
{
	public class RoutesResponse : ListResponse<Route>
	{
		[JsonProperty(PropertyName = "routes")]
		public new List<Route> Result
		{
			get { return base.Result; }
			set { base.Result = value; }
		}
	}
}