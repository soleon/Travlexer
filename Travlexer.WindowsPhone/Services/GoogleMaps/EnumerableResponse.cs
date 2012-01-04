using System.Collections.Generic;
using Newtonsoft.Json;

namespace Travlexer.WindowsPhone.Services.GoogleMaps
{
	public class EnumerableResponse<T> : ResponseBase
	{
		[JsonProperty(PropertyName = "results")]
		public virtual IList<T> Results { get; set; }
	}
}