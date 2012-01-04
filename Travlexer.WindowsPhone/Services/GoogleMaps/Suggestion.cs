using System.Collections.Generic;
using Newtonsoft.Json;

namespace Travlexer.WindowsPhone.Services.GoogleMaps
{
	public class Suggestion
	{
		[JsonProperty(PropertyName = "description")]
		public string Description { get; set; }

		[JsonProperty(PropertyName = "reference")]
		public string Reference { get; set; }

		[JsonProperty(PropertyName = "types")]
		public IList<PlaceType> Types { get; set; }
	}
}