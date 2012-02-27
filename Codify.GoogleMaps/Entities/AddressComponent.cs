using System.Collections.Generic;
using Newtonsoft.Json;

namespace Codify.GoogleMaps.Entities
{
	public class AddressComponent
	{
		[JsonProperty(PropertyName = "long_name")]
		public string LongName { get; set; }

		[JsonProperty(PropertyName = "short_name")]
		public string ShortName { get; set; }

		[JsonProperty(PropertyName = "types")]
		public IList<string> Types { get; set; }
	}
}