using Newtonsoft.Json;

namespace Codify.GoogleMaps.Entities
{
	public struct Duration
	{
		[JsonProperty(PropertyName = "text")]
		public string Text { get; set; }

		[JsonProperty(PropertyName = "value")]
		public int Value { get; set; }
	}
}