using System.Collections.Generic;
using Newtonsoft.Json;

namespace Travlexer.WindowsPhone.Services.GoogleMaps
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