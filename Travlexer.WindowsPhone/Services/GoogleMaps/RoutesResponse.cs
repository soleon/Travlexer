using System.Collections.Generic;
using Newtonsoft.Json;

namespace Travlexer.WindowsPhone.Services.GoogleMaps
{
	public class RoutesResponse : ListResponse<Route>
	{
		[JsonProperty(PropertyName = "routes")]
		public override List<Route> Results
		{
			get { return base.Results; }
			set { base.Results = value; }
		}
	}
}