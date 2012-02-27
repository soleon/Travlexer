using System.Collections.Generic;
using Newtonsoft.Json;

namespace Codify.GoogleMaps.Entities
{
	public class Place
	{
		[JsonProperty(PropertyName = "name")]
		public string Name { get; set; }

		[JsonProperty(PropertyName = "icon")]
		public string Icon { get; set; }

		[JsonProperty(PropertyName = "vicinity")]
		public string Vicinity { get; set; }

		[JsonProperty(PropertyName = "types")]
		public IList<string> Types { get; set; }

		[JsonProperty(PropertyName = "geometry")]
		public Geometry Geometry { get; set; }

		[JsonProperty(PropertyName = "reference")]
		public string Reference { get; set; }

		[JsonProperty(PropertyName = "id")]
		public string Id { get; set; }
	}
}