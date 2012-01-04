using System.Collections.Generic;
using Newtonsoft.Json;

namespace Travlexer.WindowsPhone.Services.GoogleMaps
{
	public class RoutesResponse : EnumerableResponse<Route>
	{
		[JsonProperty(PropertyName = "routes")]
		public override IList<Route> Results
		{
			get { return base.Results; }
			set { base.Results = value; }
		}
	}
}